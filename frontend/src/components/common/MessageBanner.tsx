interface MessageBannerProps {
  type: 'error' | 'success'
  message: string | null
}

function MessageBanner({ type, message }: MessageBannerProps) {
  if (!message) {
    return null
  }

  return <p className={`message ${type}`}>{message}</p>
}

export default MessageBanner
