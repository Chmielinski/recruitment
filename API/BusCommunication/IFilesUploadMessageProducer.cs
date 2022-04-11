namespace API.BusCommunication
{
    public interface IFilesUploadMessageProducer
    {
        void SendMessage<T>(T message);
    }
}
