﻿using Datamarka_DomainModel.Models.ECommerce;
using Datamarka_DomainModel.Models.Identity;

namespace Datamarka_BLL.Contracts
{
    public class OrderBriefModel
    {
        public long? Id { get; set; }
        public string? OrderCode { get; set; }
        public User? User { get; set; }
        public string? UserId { get; set; }
        public Product? Product { get; set; }
        public long ProductId { get; set; }
        public required DateTime ProdDate {get; set;}
        public required DateTime WarrantDate { get; set;}
        public required OrderStatusEnum Status { get; set; }
    }
}
