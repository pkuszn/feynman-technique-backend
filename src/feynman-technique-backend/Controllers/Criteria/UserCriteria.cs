namespace FeynmanTechniqueBackend.Controllers.Criteria
{
    public class UserCriteria
    {
        public int IdUser { get; set; }
        public int Role { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
