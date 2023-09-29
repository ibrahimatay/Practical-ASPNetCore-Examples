using System.Runtime.Serialization;
using CoreWCF;

namespace server;

[ServiceContract]
public interface IEchoService
{
    [OperationContract]
    string Echo(string text);

    [OperationContract]
    string ComplexEcho(EchoMessage text);
}

[DataContract]
public class EchoMessage
{
    [DataMember]
    public string Text { get; set; }
}
