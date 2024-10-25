namespace FeynmanTechniqueBackend.Controllers.Criteria
{
    public class UserCriteria
    {
        public int IdUser { get; set; }
        public int Role { get; set; }
        public string Name { get; set; } = null!;
        public string Password { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
    }
}
