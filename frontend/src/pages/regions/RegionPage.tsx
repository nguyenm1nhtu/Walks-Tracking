import { isAxiosError } from 'axios'
import { Compass, MapPinned, Route as RouteIcon, Ruler } from 'lucide-react'
import { useEffect, useMemo, useState } from 'react'
import type { FormEvent } from 'react'
import MessageBanner from '../../components/common/MessageBanner'
import RegionFormFields from '../../components/regions/RegionFormFields'
import WalkFormFields from '../../components/walks/WalkFormFields'
import AuthPage from '../auth/AuthPage'
import { clearAccessToken, getAccessToken } from '../../lib/apiClient'
import {
  createRegion,
  deleteRegion,
  getRegionById,
  getRegions,
  updateRegion,
} from '../../services/regionService'
import {
  createWalk,
  deleteWalk,
  getWalkById,
  getWalks,
  updateWalk,
} from '../../services/walkService'
import { getDifficulties } from '../../services/difficultyService'
import { uploadImage } from '../../services/imageService'
import type { AddRegionDto, RegionDto, UpdateRegionDto } from '../../types/region'
import type { AddWalkDto, DifficultyDto, WalkDto, WalkFormValues } from '../../types/walk'

type ManagementTab = 'region' | 'walk'

const emptyRegion: AddRegionDto = {
  name: '',
  code: '',
  regionImageUrl: '',
}

const emptyUpdateRegion: UpdateRegionDto = {
  name: '',
  code: '',
  regionImageUrl: '',
}

const emptyWalkForm: WalkFormValues = {
  name: '',
  description: '',
  lengthInKm: '0',
  walkImageUrl: '',
  regionId: '',
  difficultyId: '',
}

function RegionPage() {
  const [regions, setRegions] = useState<RegionDto[]>([])
  const [selectedRegion, setSelectedRegion] = useState<RegionDto | null>(null)
  const [selectedRegionId, setSelectedRegionId] = useState('')
  const [newRegion, setNewRegion] = useState<AddRegionDto>(emptyRegion)
  const [editRegion, setEditRegion] = useState<UpdateRegionDto>(emptyUpdateRegion)
  const [isRegionLoading, setIsRegionLoading] = useState(false)

  const [walks, setWalks] = useState<WalkDto[]>([])
  const [difficulties, setDifficulties] = useState<DifficultyDto[]>([])
  const [selectedWalk, setSelectedWalk] = useState<WalkDto | null>(null)
  const [selectedWalkId, setSelectedWalkId] = useState('')
  const [newWalk, setNewWalk] = useState<WalkFormValues>(emptyWalkForm)
  const [editWalk, setEditWalk] = useState<WalkFormValues>(emptyWalkForm)
  const [isWalkLoading, setIsWalkLoading] = useState(false)

  const [createWalkImageFile, setCreateWalkImageFile] = useState<File | null>(null)
  const [updateWalkImageFile, setUpdateWalkImageFile] = useState<File | null>(null)
  const [createWalkImageStatus, setCreateWalkImageStatus] = useState('No image selected yet.')
  const [updateWalkImageStatus, setUpdateWalkImageStatus] = useState('Keep current image / none.')
  const [isCreateWalkImageUploading, setIsCreateWalkImageUploading] = useState(false)
  const [isUpdateWalkImageUploading, setIsUpdateWalkImageUploading] = useState(false)
  const [createWalkImageInputKey, setCreateWalkImageInputKey] = useState(0)
  const [updateWalkImageInputKey, setUpdateWalkImageInputKey] = useState(0)

  const [walkKeyword, setWalkKeyword] = useState('')
  const [activeTab, setActiveTab] = useState<ManagementTab>('region')

  const [errorMessage, setErrorMessage] = useState<string | null>(null)
  const [successMessage, setSuccessMessage] = useState<string | null>(null)

  useEffect(() => {
    void handleAuthChanged()
  }, [])

  const isAuthenticated = hasAccessToken()

  const filteredWalks = useMemo(() => {
    const keyword = walkKeyword.trim().toLowerCase()

    if (!keyword) {
      return walks
    }

    return walks.filter(
      (walk) =>
        walk.name.toLowerCase().includes(keyword) ||
        walk.region.name.toLowerCase().includes(keyword) ||
        walk.difficulty.name.toLowerCase().includes(keyword),
    )
  }, [walks, walkKeyword])

  const difficultyOptions = useMemo<DifficultyDto[]>(() => {
    if (difficulties.length > 0) {
      return difficulties
    }

    const options = new Map<string, string>()
    for (const walk of walks) {
      if (!options.has(walk.difficulty.id)) {
        options.set(walk.difficulty.id, walk.difficulty.name)
      }
    }

    return Array.from(options, ([id, name]) => ({ id, name }))
  }, [difficulties, walks])

  const totalDistance = useMemo(
    () => walks.reduce((sum, walk) => sum + (Number.isFinite(walk.lengthInKm) ? walk.lengthInKm : 0), 0),
    [walks],
  )

  const selectedWalkPreview = selectedWalk ?? walks.find((walk) => walk.id === selectedWalkId) ?? null

  useEffect(() => {
    if (regions.length === 0) {
      return
    }

    setNewWalk((current) =>
      current.regionId ? current : { ...current, regionId: regions[0].id },
    )
    setEditWalk((current) =>
      current.regionId ? current : { ...current, regionId: regions[0].id },
    )
  }, [regions])

  useEffect(() => {
    if (difficultyOptions.length === 0) {
      return
    }

    setNewWalk((current) =>
      current.difficultyId ? current : { ...current, difficultyId: difficultyOptions[0].id },
    )
    setEditWalk((current) =>
      current.difficultyId ? current : { ...current, difficultyId: difficultyOptions[0].id },
    )
  }, [difficultyOptions])

  async function handleAuthChanged() {
    setErrorMessage(null)
    setSuccessMessage(null)

    if (!hasAccessToken()) {
      setRegions([])
      setWalks([])
      setDifficulties([])
      setSelectedRegion(null)
      setSelectedWalk(null)
      return
    }

    await Promise.all([refreshRegions(), refreshWalks(), refreshDifficulties()])
  }

  async function refreshRegions() {
    if (!hasAccessToken()) {
      setRegions([])
      return
    }

    setIsRegionLoading(true)
    setErrorMessage(null)

    try {
      const response = await getRegions()
      setRegions(response)
    } catch (error) {
      if (isUnauthorized(error)) {
        clearAccessToken()
        setRegions([])
        setWalks([])
        setDifficulties([])
        setErrorMessage('Session expired. Please sign in again.')
      } else {
        setErrorMessage(toUserMessage(error))
      }
    } finally {
      setIsRegionLoading(false)
    }
  }

  async function refreshWalks() {
    if (!hasAccessToken()) {
      setWalks([])
      return
    }

    setIsWalkLoading(true)
    setErrorMessage(null)

    try {
      const response = await getWalks()
      setWalks(response)
    } catch (error) {
      if (isUnauthorized(error)) {
        clearAccessToken()
        setRegions([])
        setWalks([])
        setDifficulties([])
        setErrorMessage('Session expired. Please sign in again.')
      } else {
        setErrorMessage(toUserMessage(error))
      }
    } finally {
      setIsWalkLoading(false)
    }
  }

  async function refreshDifficulties() {
    if (!hasAccessToken()) {
      setDifficulties([])
      return
    }

    try {
      const response = await getDifficulties()
      setDifficulties(response)
    } catch (error) {
      if (isUnauthorized(error)) {
        clearAccessToken()
        setRegions([])
        setWalks([])
        setDifficulties([])
        setErrorMessage('Session expired. Please sign in again.')
      } else if (isAxiosError(error) && error.response?.status === 404) {
        setDifficulties([])
        setErrorMessage('Difficulty API not found. Restart backend and try again.')
      } else {
        setErrorMessage(toUserMessage(error))
      }
    }
  }

  async function handleGetRegionById(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setErrorMessage(null)
    setSuccessMessage(null)

    if (!selectedRegionId.trim()) {
      setErrorMessage('Please select a region.')
      return
    }

    try {
      const region = await getRegionById(selectedRegionId.trim())
      setSelectedRegion(region)
      setEditRegion({
        name: region.name,
        code: region.code,
        regionImageUrl: region.regionImageUrl ?? '',
      })
    } catch (error) {
      setSelectedRegion(null)
      setErrorMessage(toUserMessage(error))
    }
  }

  async function handleCreateRegion(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setErrorMessage(null)
    setSuccessMessage(null)

    try {
      const created = await createRegion({
        name: newRegion.name.trim(),
        code: newRegion.code.trim(),
        regionImageUrl: normalizeOptionalString(newRegion.regionImageUrl),
      })

      setSuccessMessage(`Region created successfully: ${created.id}`)
      setNewRegion(emptyRegion)
      await refreshRegions()
    } catch (error) {
      setErrorMessage(toUserMessage(error))
    }
  }

  async function handleUpdateRegion(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setErrorMessage(null)
    setSuccessMessage(null)

    if (!selectedRegionId.trim()) {
      setErrorMessage('Please select a region to update.')
      return
    }

    try {
      const updated = await updateRegion(selectedRegionId.trim(), {
        name: editRegion.name.trim(),
        code: editRegion.code.trim(),
        regionImageUrl: normalizeOptionalString(editRegion.regionImageUrl),
      })

      setSelectedRegion(updated)
      setSuccessMessage(`Region updated successfully: ${updated.id}`)
      await refreshRegions()
    } catch (error) {
      setErrorMessage(toUserMessage(error))
    }
  }

  async function handleDeleteRegion() {
    setErrorMessage(null)
    setSuccessMessage(null)

    if (!selectedRegionId.trim()) {
      setErrorMessage('Please select a region to delete.')
      return
    }

    try {
      await deleteRegion(selectedRegionId.trim())
      setSelectedRegion(null)
      setEditRegion(emptyUpdateRegion)
      setSuccessMessage(`Region deleted: ${selectedRegionId.trim()}`)
      await refreshRegions()
    } catch (error) {
      setErrorMessage(toUserMessage(error))
    }
  }

  async function handleGetWalkById(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setErrorMessage(null)
    setSuccessMessage(null)

    if (!selectedWalkId.trim()) {
      setErrorMessage('Please select a walk.')
      return
    }

    try {
      const walk = await getWalkById(selectedWalkId.trim())
      setSelectedWalk(walk)
      setEditWalk(mapWalkToFormValues(walk))
      setUpdateWalkImageStatus(
        walk.walkImageUrl ? 'Using current walk image. Upload to replace.' : 'Keep current image / none.',
      )
      setUpdateWalkImageFile(null)
      setUpdateWalkImageInputKey((current) => current + 1)
    } catch (error) {
      setSelectedWalk(null)
      setErrorMessage(toUserMessage(error))
    }
  }

  async function handleCreateWalk(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setErrorMessage(null)
    setSuccessMessage(null)

    const payload = toWalkPayload(newWalk)
    if (!payload) {
      return
    }

    try {
      const created = await createWalk(payload)
      setSuccessMessage(`Walk created successfully: ${created.id}`)
      setNewWalk((current) => ({
        ...emptyWalkForm,
        regionId: current.regionId,
        difficultyId: current.difficultyId,
        walkImageUrl: '',
      }))
      setCreateWalkImageStatus('No image selected yet.')
      setCreateWalkImageFile(null)
      setCreateWalkImageInputKey((current) => current + 1)
      await refreshWalks()
    } catch (error) {
      setErrorMessage(toUserMessage(error))
    }
  }

  async function handleUpdateWalk(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setErrorMessage(null)
    setSuccessMessage(null)

    if (!selectedWalkId.trim()) {
      setErrorMessage('Please select a walk to update.')
      return
    }

    const payload = toWalkPayload(editWalk)
    if (!payload) {
      return
    }

    try {
      const updated = await updateWalk(selectedWalkId.trim(), payload)
      setSelectedWalk(updated)
      setSuccessMessage(`Walk updated successfully: ${updated.id}`)
      await refreshWalks()
    } catch (error) {
      setErrorMessage(toUserMessage(error))
    }
  }

  async function handleDeleteWalk() {
    setErrorMessage(null)
    setSuccessMessage(null)

    if (!selectedWalkId.trim()) {
      setErrorMessage('Please select a walk to delete.')
      return
    }

    try {
      await deleteWalk(selectedWalkId.trim())
      setSelectedWalk(null)
      setEditWalk((current) => ({
        ...emptyWalkForm,
        regionId: current.regionId,
        difficultyId: current.difficultyId,
      }))
      setUpdateWalkImageStatus('Keep current image / none.')
      setUpdateWalkImageFile(null)
      setUpdateWalkImageInputKey((current) => current + 1)
      setSuccessMessage(`Walk deleted: ${selectedWalkId.trim()}`)
      await refreshWalks()
    } catch (error) {
      setErrorMessage(toUserMessage(error))
    }
  }

  async function handleUploadWalkImage(mode: 'create' | 'update') {
    setErrorMessage(null)
    setSuccessMessage(null)

    const file = mode === 'create' ? createWalkImageFile : updateWalkImageFile
    if (!file) {
      setErrorMessage('Please choose an image file before uploading.')
      return
    }

    if (mode === 'create') {
      setIsCreateWalkImageUploading(true)
    } else {
      setIsUpdateWalkImageUploading(true)
    }

    try {
      const fileName = deriveFileNameWithoutExtension(file.name).trim()
      const uploaded = await uploadImage({
        file,
        fileName: fileName || 'walk-image',
        fileDescription: null,
      })

      if (mode === 'create') {
        setNewWalk((current) => ({ ...current, walkImageUrl: uploaded.filePath }))
        setCreateWalkImageStatus(`Image selected: ${uploaded.fileName}`)
        setCreateWalkImageFile(null)
        setCreateWalkImageInputKey((current) => current + 1)
      } else {
        setEditWalk((current) => ({ ...current, walkImageUrl: uploaded.filePath }))
        setUpdateWalkImageStatus(`Image selected: ${uploaded.fileName}`)
        setUpdateWalkImageFile(null)
        setUpdateWalkImageInputKey((current) => current + 1)
      }

      setSuccessMessage(`Image uploaded successfully: ${uploaded.fileName}`)
    } catch (error) {
      setErrorMessage(toUserMessage(error))
    } finally {
      if (mode === 'create') {
        setIsCreateWalkImageUploading(false)
      } else {
        setIsUpdateWalkImageUploading(false)
      }
    }
  }

  function handleCreateWalkImageFileChange(file: File | null) {
    setCreateWalkImageFile(file)
    setCreateWalkImageStatus(file ? `Selected file: ${file.name}` : 'No image selected yet.')
  }

  function handleUpdateWalkImageFileChange(file: File | null) {
    setUpdateWalkImageFile(file)
    setUpdateWalkImageStatus(file ? `Selected file: ${file.name}` : 'Keep current image / none.')
  }

  function handleWalkSelect(walk: WalkDto) {
    setSelectedWalk(walk)
    setSelectedWalkId(walk.id)
    setEditWalk(mapWalkToFormValues(walk))
    setUpdateWalkImageStatus(
      walk.walkImageUrl ? 'Using current walk image. Upload to replace.' : 'Keep current image / none.',
    )
    setUpdateWalkImageFile(null)
    setUpdateWalkImageInputKey((current) => current + 1)
  }

  function handleRegionSelect(region: RegionDto) {
    setSelectedRegion(region)
    setSelectedRegionId(region.id)
    setEditRegion({
      name: region.name,
      code: region.code,
      regionImageUrl: region.regionImageUrl ?? '',
    })
  }

  function toWalkPayload(values: WalkFormValues): AddWalkDto | null {
    const lengthInKm = Number(values.lengthInKm)
    if (Number.isNaN(lengthInKm)) {
      setErrorMessage('LengthInKm must be a valid number.')
      return null
    }

    if (!values.regionId.trim() || !values.difficultyId.trim()) {
      setErrorMessage('RegionId and DifficultyId are required for Walk.')
      return null
    }

    return {
      name: values.name.trim(),
      description: values.description.trim(),
      lengthInKm,
      walkImageUrl: normalizeOptionalString(values.walkImageUrl),
      regionId: values.regionId.trim(),
      difficultyId: values.difficultyId.trim(),
    }
  }

  return (
    <main className="tracking-app">
      <header className="tracking-header">
        <div className="tracking-brand">
          <span className="tracking-logo" aria-hidden="true">
            <RouteIcon size={24} />
          </span>
          <div>
            <p className="tracking-kicker">Trail Management</p>
            <h1>Walk Tracking</h1>
            <p className="tracking-subtitle">Plan, manage and monitor your regions, walks and media.</p>
          </div>
        </div>

        <AuthPage onAuthChanged={() => void handleAuthChanged()} />
      </header>

      <MessageBanner type="error" message={errorMessage} />
      <MessageBanner type="success" message={successMessage} />

      <section className="stats-grid">
        <article className="stat-card">
          <span className="stat-icon">
            <MapPinned size={18} />
          </span>
          <p className="stat-label">Regions</p>
          <p className="stat-value">{regions.length}</p>
        </article>

        <article className="stat-card">
          <span className="stat-icon">
            <Compass size={18} />
          </span>
          <p className="stat-label">Walks</p>
          <p className="stat-value">{walks.length}</p>
        </article>

        <article className="stat-card">
          <span className="stat-icon">
            <Ruler size={18} />
          </span>
          <p className="stat-label">Distance</p>
          <p className="stat-value">{totalDistance.toFixed(1)} km</p>
        </article>
      </section>

      <section className="explorer-grid">
        <article className="panel">
          <div className="panel-header">
            <div>
              <h2>Explore Walks</h2>
              <p>Live list from /api/Walk</p>
            </div>
            <button type="button" onClick={() => void refreshWalks()} disabled={isWalkLoading || !isAuthenticated}>
              {isWalkLoading ? 'Refreshing...' : 'Refresh'}
            </button>
          </div>

          <input
            className="input"
            type="text"
            placeholder="Search by walk, region or difficulty"
            value={walkKeyword}
            onChange={(event) => setWalkKeyword(event.target.value)}
          />

          <div className="walk-feed">
            {filteredWalks.length === 0 ? (
              <p className="empty-note">No walks found.</p>
            ) : (
              filteredWalks.map((walk) => (
                <button
                  key={walk.id}
                  type="button"
                  className={`walk-item ${selectedWalkId === walk.id ? 'is-active' : ''}`}
                  onClick={() => handleWalkSelect(walk)}
                >
                  <div className="walk-item-top">
                    <strong>{walk.name}</strong>
                    <span>{walk.lengthInKm} km</span>
                  </div>
                  <div className="walk-item-meta">
                    <span>{walk.region.name}</span>
                    <span>{walk.difficulty.name}</span>
                  </div>
                </button>
              ))
            )}
          </div>
        </article>

        <article className="panel">
          <div className="panel-header">
            <div>
              <h2>Walk Details</h2>
              <p>Current selection</p>
            </div>
          </div>

          {selectedWalkPreview ? (
            <div className="detail-list">
              <div className="detail-item">
                <span>Name</span>
                <strong>{selectedWalkPreview.name}</strong>
              </div>
              <div className="detail-item">
                <span>Region</span>
                <strong>{selectedWalkPreview.region.name}</strong>
              </div>
              <div className="detail-item">
                <span>Difficulty</span>
                <strong>{selectedWalkPreview.difficulty.name}</strong>
              </div>
              <div className="detail-item">
                <span>Length</span>
                <strong>{selectedWalkPreview.lengthInKm} km</strong>
              </div>
              <div className="detail-item">
                <span>Description</span>
                <p>{selectedWalkPreview.description}</p>
              </div>
            </div>
          ) : (
            <p className="empty-note">Pick a walk from the list to see detail.</p>
          )}
        </article>
      </section>

      <section className="panel">
        <div className="panel-header">
          <div>
            <h2>Regions</h2>
            <p>Quick pick from /api/Region</p>
          </div>
          <button type="button" onClick={() => void refreshRegions()} disabled={isRegionLoading || !isAuthenticated}>
            {isRegionLoading ? 'Refreshing...' : 'Refresh'}
          </button>
        </div>

        <div className="region-chips">
          {regions.length === 0 ? (
            <p className="empty-note">No regions loaded.</p>
          ) : (
            regions.map((region) => (
              <button
                key={region.id}
                type="button"
                className={`region-chip ${selectedRegionId === region.id ? 'is-active' : ''}`}
                onClick={() => handleRegionSelect(region)}
              >
                {region.name}
                <span>{region.code}</span>
              </button>
            ))
          )}
        </div>
      </section>

      <section className="management">
        <div className="management-header">
          <div>
            <h2>Management Studio</h2>
            <p>CRUD operations for Region and Walk</p>
          </div>
          <div className="management-tabs" role="tablist" aria-label="Management tabs">
            <button
              type="button"
              className={`tab-button ${activeTab === 'region' ? 'is-active' : ''}`}
              onClick={() => setActiveTab('region')}
            >
              Region
            </button>
            <button
              type="button"
              className={`tab-button ${activeTab === 'walk' ? 'is-active' : ''}`}
              onClick={() => setActiveTab('walk')}
            >
              Walk
            </button>
          </div>
        </div>

        {activeTab === 'region' && (
          <div className="management-grid">
            <article className="panel">
              <h3>Find Region</h3>
              <form onSubmit={handleGetRegionById} className="column">
                <select
                  className="input"
                  value={selectedRegionId}
                  onChange={(event) => setSelectedRegionId(event.target.value)}
                >
                  <option value="">Select region</option>
                  {regions.map((region) => (
                    <option key={region.id} value={region.id}>
                      {region.name} ({region.code})
                    </option>
                  ))}
                </select>
                <button type="submit">Load region</button>
              </form>

              {selectedRegion && <pre className="json-box">{JSON.stringify(selectedRegion, null, 2)}</pre>}
            </article>

            <article className="panel">
              <h3>Create Region</h3>
              <form onSubmit={handleCreateRegion} className="column">
                <RegionFormFields value={newRegion} onChange={(next) => setNewRegion(next)} />
                <button type="submit">Create region</button>
              </form>
            </article>

            <article className="panel">
              <h3>Update / Delete Region</h3>
              <form onSubmit={handleUpdateRegion} className="column">
                <RegionFormFields value={editRegion} onChange={(next) => setEditRegion(next)} />
                <button type="submit">Update selected region</button>
              </form>

              <button type="button" className="danger" onClick={() => void handleDeleteRegion()}>
                Delete selected region
              </button>
            </article>
          </div>
        )}

        {activeTab === 'walk' && (
          <div className="management-grid">
            <article className="panel">
              <h3>Find Walk</h3>
              <form onSubmit={handleGetWalkById} className="column">
                <select
                  className="input"
                  value={selectedWalkId}
                  onChange={(event) => setSelectedWalkId(event.target.value)}
                >
                  <option value="">Select walk</option>
                  {walks.map((walk) => (
                    <option key={walk.id} value={walk.id}>
                      {walk.name} ({walk.lengthInKm} km)
                    </option>
                  ))}
                </select>
                <button type="submit">Load walk</button>
              </form>

              {selectedWalk && <pre className="json-box">{JSON.stringify(selectedWalk, null, 2)}</pre>}
            </article>

            <article className="panel">
              <h3>Create Walk</h3>
              <form onSubmit={handleCreateWalk} className="column">
                <WalkFormFields
                  value={newWalk}
                  regions={regions}
                  difficulties={difficultyOptions}
                  onChange={(next) => setNewWalk(next)}
                  imageControl={{
                    fileInputKey: createWalkImageInputKey,
                    isUploading: isCreateWalkImageUploading,
                    statusText: createWalkImageStatus,
                    onFileChange: handleCreateWalkImageFileChange,
                    onUpload: () => {
                      void handleUploadWalkImage('create')
                    },
                  }}
                />
                <button type="submit">Create walk</button>
              </form>
            </article>

            <article className="panel">
              <h3>Update / Delete Walk</h3>
              <form onSubmit={handleUpdateWalk} className="column">
                <WalkFormFields
                  value={editWalk}
                  regions={regions}
                  difficulties={difficultyOptions}
                  onChange={(next) => setEditWalk(next)}
                  imageControl={{
                    fileInputKey: updateWalkImageInputKey,
                    isUploading: isUpdateWalkImageUploading,
                    statusText: updateWalkImageStatus,
                    onFileChange: handleUpdateWalkImageFileChange,
                    onUpload: () => {
                      void handleUploadWalkImage('update')
                    },
                  }}
                />
                <button type="submit">Update selected walk</button>
              </form>

              <button type="button" className="danger" onClick={() => void handleDeleteWalk()}>
                Delete selected walk
              </button>
            </article>
          </div>
        )}
      </section>
    </main>
  )
}

function hasAccessToken() {
  return Boolean(getAccessToken().trim())
}

function isUnauthorized(error: unknown): boolean {
  return isAxiosError(error) && error.response?.status === 401
}

function deriveFileNameWithoutExtension(fileName: string): string {
  const dotIndex = fileName.lastIndexOf('.')
  if (dotIndex <= 0) {
    return fileName
  }

  return fileName.slice(0, dotIndex)
}

function mapWalkToFormValues(walk: WalkDto): WalkFormValues {
  return {
    name: walk.name,
    description: walk.description,
    lengthInKm: walk.lengthInKm.toString(),
    walkImageUrl: walk.walkImageUrl ?? '',
    regionId: walk.region.id,
    difficultyId: walk.difficulty.id,
  }
}

function normalizeOptionalString(value: string | null | undefined) {
  const trimmed = value?.trim()
  return trimmed ? trimmed : null
}

function toUserMessage(error: unknown): string {
  if (isAxiosError(error)) {
    if (error.response?.status === 401) {
      return 'Please sign in to continue.'
    }

    const serverData = error.response?.data

    if (typeof serverData === 'string' && serverData.trim()) {
      return serverData
    }

    if (serverData && typeof serverData === 'object') {
      const typedData = serverData as {
        detail?: unknown
        title?: unknown
        errors?: Record<string, string[]>
      }
      const detail = typeof typedData.detail === 'string' ? typedData.detail : ''
      const title = typeof typedData.title === 'string' ? typedData.title : ''

      if (title || detail) {
        return `${title}${title && detail ? ': ' : ''}${detail}`
      }

      if (typedData.errors && typeof typedData.errors === 'object') {
        for (const value of Object.values(typedData.errors)) {
          if (Array.isArray(value) && value.length > 0 && typeof value[0] === 'string') {
            return value[0]
          }
        }
      }
    }

    if (error.message) {
      return error.message
    }
  }

  if (error instanceof Error) {
    return error.message
  }

  return 'An error occurred while calling the API.'
}

export default RegionPage
