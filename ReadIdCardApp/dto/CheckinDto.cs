using ReadIdCardApp.db;

namespace ReadIdCardApp.dto;

public class CheckinDto {
    public int Id { get; set; }
    public int ScheduleId { get; set; }
    public People? People { get; set; }
    public int Type { get; set; }
    public long CheckinTime { get; set; }
}