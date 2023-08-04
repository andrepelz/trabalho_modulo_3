namespace TrabalhoModuloTres.Shared;

public interface IMessage
{
    DateTime Date { get; }
    ICollection<NotificationDto> Message { get; }
    string Author { get; }
}