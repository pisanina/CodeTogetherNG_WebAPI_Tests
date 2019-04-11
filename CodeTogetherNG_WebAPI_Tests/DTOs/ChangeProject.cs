namespace CodeTogetherNG_WebAPI_Tests.DTOs
{
    internal class ChangeProject : AddProject
    {
        public int State { get; set; }
        public int ProjectId { get; set; }
    }
}