using System;
using System.Runtime.Serialization;

namespace GRPC.Demo.WcfService.DataContract
{
    [DataContract]
    public class ScrittoreWcf
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string Nome { get; set; }

        [DataMember]
        public string Cognome { get; set; }

    }
}