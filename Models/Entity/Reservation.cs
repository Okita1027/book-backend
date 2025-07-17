namespace DEMO_CRUD.Models.Entity
{
    public class Reservation
    {
        public int Id { get; set; }

        public DateTime ReservationDate { get; set; } = DateTime.UtcNow; // 预约日期
        public ReservationStatus Status { get; set; } = ReservationStatus.Active; // 预约状态

        // --- 导航属性 ---
        public int BookId { get; set; }
        public Book Book { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }

    public enum ReservationStatus
    {
        Active,     // 正在预约中
        Fulfilled,  // 已满足（已通知或已借阅）
        Canceled    // 已取消
    }
}
