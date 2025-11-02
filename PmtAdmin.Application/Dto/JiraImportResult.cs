namespace PmtAdmin.Application.Dto
{
    public class JiraImportResult
    {
        public List<UserDto> Users { get; set; }

        public List<ProjectDTO> Projects { get; set; }

        public bool ImportFailed { get; set; }
        public List<OperationResult> Results { get; internal set; }
    }
}
