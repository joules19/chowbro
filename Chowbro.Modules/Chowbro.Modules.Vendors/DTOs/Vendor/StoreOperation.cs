using Chowbro.Core.Entities.Vendor;

public class StoreOperationDto
{
    public DeliveryType DeliveryType { get; set; }
    public string? OrderCutoffTime { get; set; } // "HH:mm" format
    public string? MenuReadyTime { get; set; } // "HH:mm" format
    public List<OpeningHoursDto> OpeningHours { get; set; } = new();
}

public class OpeningHoursDto
{
    public DayOfWeek Day { get; set; }
    public string OpenTime { get; set; } // "HH:mm" format
    public string CloseTime { get; set; } // "HH:mm" format
    public bool IsClosed { get; set; }
}