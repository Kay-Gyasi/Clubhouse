﻿namespace Clubhouse.Data.Entities.Base;

public class Entity
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string CreatedBy { get; set; } = "sysadmin";
    public string CreatedById { get; set; }
    public DateTime CreatedAt { get; set; }
    public string UpdatedBy { get; set; } = "sysadmin";
    public string UpdatedById { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}