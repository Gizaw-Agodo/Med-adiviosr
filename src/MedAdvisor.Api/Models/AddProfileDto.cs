namespace MedAdvisor.Api.Dtos
{
    public class AddProfileDto
    {
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public int CPRNumber { get; set; }
        public string? Nationality { get; set; }
        public String TelephoneNumber { get; set; } = string.Empty;
        public bool OrganDonor { get; set; }
        public string? Zip { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? StreetAddress { get; set; }
        public string? InsuranceType { get; set; }
        public string? InsuranceCompany { get; set; }
        public string? PolicyNumber { get; set; }
        public string? EmergencyPhone { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhoneNo { get; set; }
        public string? Relationship { get; set; }
    }
}
