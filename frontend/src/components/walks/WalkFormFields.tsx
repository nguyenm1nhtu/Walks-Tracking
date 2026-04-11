import type { RegionDto } from '../../types/region'
import type { DifficultyDto, WalkFormValues } from '../../types/walk'

interface WalkImageControl {
  fileInputKey: number
  isUploading: boolean
  statusText: string
  onFileChange: (file: File | null) => void
  onUpload: () => void
}

interface WalkFormFieldsProps {
  value: WalkFormValues
  regions: RegionDto[]
  difficulties: DifficultyDto[]
  onChange: (next: WalkFormValues) => void
  imageControl: WalkImageControl
}

function WalkFormFields({ value, regions, difficulties, onChange, imageControl }: WalkFormFieldsProps) {
  return (
    <>
      <input
        className="input"
        type="text"
        placeholder="Name"
        value={value.name}
        onChange={(event) => onChange({ ...value, name: event.target.value })}
      />
      <textarea
        className="input"
        placeholder="Description"
        value={value.description}
        onChange={(event) => onChange({ ...value, description: event.target.value })}
      />
      <input
        className="input"
        type="number"
        min="0"
        step="0.1"
        placeholder="LengthInKm"
        value={value.lengthInKm}
        onChange={(event) => onChange({ ...value, lengthInKm: event.target.value })}
      />
      <select
        className="input"
        value={value.regionId}
        onChange={(event) => onChange({ ...value, regionId: event.target.value })}
      >
        <option value="">Select region</option>
        {regions.map((region) => (
          <option key={region.id} value={region.id}>
            {region.name} ({region.code})
          </option>
        ))}
      </select>
      <select
        className="input"
        value={value.difficultyId}
        onChange={(event) => onChange({ ...value, difficultyId: event.target.value })}
      >
        <option value="">Select difficulty</option>
        {difficulties.map((difficulty) => (
          <option key={difficulty.id} value={difficulty.id}>
            {difficulty.name}
          </option>
        ))}
      </select>

      <div className="column walk-image-block">
        <input
          key={imageControl.fileInputKey}
          className="input"
          type="file"
          accept=".jpg,.jpeg,.png,image/jpeg,image/png"
          onChange={(event) => imageControl.onFileChange(event.target.files?.[0] ?? null)}
        />
        <button type="button" className="secondary" onClick={imageControl.onUpload} disabled={imageControl.isUploading}>
          {imageControl.isUploading ? 'Uploading image...' : 'Upload walk image'}
        </button>
        <p className="helper-text">{imageControl.statusText}</p>
      </div>
    </>
  )
}

export default WalkFormFields
