using System.ServiceModel;
using client;

const string serviceUrl = @"http://localhost:8080/echo";
SimpleEcho();
ComplexEcho();

void SimpleEcho()
{
    var factory = new ChannelFactory<IEchoService>(new BasicHttpBinding(), new EndpointAddress(serviceUrl));
    factory.Open();
    var channel = factory.CreateChannel();
    ((IClientChannel)channel).Open();
    Console.WriteLine("http Echo(\"Hello\") => " + channel.Echo("Hello"));
    ((IClientChannel)channel).Close();
    factory.Close();
}

void ComplexEcho()
{
    var factory = new ChannelFactory<IEchoService>(new BasicHttpBinding(), new EndpointAddress(serviceUrl));
    factory.Open();
    var channel = factory.CreateChannel();
    ((IClientChannel)channel).Open();
    Console.WriteLine("http EchoMessage(\"Complex Hello\") => " + channel.ComplexEcho(new EchoMessage() { Text = "Complex Hello" }));
    ((IClientChannel)channel).Close();
    factory.Close();
}

