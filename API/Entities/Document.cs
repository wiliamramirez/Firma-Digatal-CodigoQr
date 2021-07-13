using System;
using System.Collections.Generic;

namespace API.Entities
{
    public class Document
    {
        public Guid Id { get; set; }//
        public string Url { get; set; }//

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;//
        
        /*Relacion con la tabla users*/

        public Guid AppUserId { get; set; }
        
        
        public AppUser AppUser { get; set; }

        //relación con documentDetails
        public DocumentDetail DocumentDetail { get; set; }
        
        
        
    }
}