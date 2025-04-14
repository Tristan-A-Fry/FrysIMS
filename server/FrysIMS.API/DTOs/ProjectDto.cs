namespace FrysIMS.API.Dtos{
  public class ProjectDto
  {
      public int Id { get; set; }
      public string Name { get; set; }
      public decimal Budget { get; set; }
      public string CreatedByUserId { get; set; }
      public DateTime DateCreated { get; set; }
      public List<ProjectMaterialDto> ProjectMaterials { get; set; }
  }
}
