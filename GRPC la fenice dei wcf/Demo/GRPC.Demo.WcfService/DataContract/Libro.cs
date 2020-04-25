using System;
using System.Runtime.Serialization;

namespace GRPC.Demo.WcfService.DataContract
{
    [DataContract]
    public class LibroWcf
    {
        [DataMember] public Guid Id { get; set; }

        [DataMember] public string Titolo { get; set; }

        [DataMember] public ScrittoreWcf Autore { get; set; }

    }
}