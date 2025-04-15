using Chowbro.Core.Entities.Vendor;

public class StoreOperationDto
{
    public DeliveryType DeliveryType { get; set; }
    public string? DeliveryTypeName { get; set; } = string.Empty;
    public string? OrderCutoffTime { get; set; } // "HH:mm" format
    public string? MenuReadyTime { get; set; } // "HH:mm" format
    public List<OpeningHoursDto> OpeningHours { get; set; } = new();
}

public class OpeningHoursDto
{
    public DayOfWeek Day { get; set; }
    public string DayName { get; set; } = string.Empty;
    public string OpenTime { get; set; } // "HH:mm" format
    public string CloseTime { get; set; } // "HH:mm" format
    public bool IsClosed { get; set; }
}