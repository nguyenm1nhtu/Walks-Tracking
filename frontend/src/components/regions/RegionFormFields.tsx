import type { AddRegionDto, UpdateRegionDto } from '../../types/region'

type RegionFormModel = AddRegionDto | UpdateRegionDto

interface RegionFormFieldsProps {
  value: RegionFormModel
  onChange: (next: RegionFormModel) => void
}

function RegionFormFields({ value, onChange }: RegionFormFieldsProps) {
  return (
    <>
      <input
        className="input"
        type="text"
        placeholder="Name"
        value={value.name}
        onChange={(event) => onChange({ ...value, name: event.target.value })}
      />
      <input
        className="input"
        type="text"
        placeholder="Code"
        value={value.code}
        onChange={(event) => onChange({ ...value, code: event.target.value })}
      />
    </>
  )
}

export default RegionFormFields
