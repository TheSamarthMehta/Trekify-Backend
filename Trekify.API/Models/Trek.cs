using System.ComponentModel.DataAnnotations;

namespace Trekify.API.Models;

public class Trek
{
    [Key]
    public int Id { get; set; }
    
    public int SerialNumber { get; set; }
    
    [Required]
    [StringLength(255)]
    public string TrekName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string State { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string TrekType { get; set; } = string.Empty;
    
    [StringLength(50)]
    public string DifficultyLevel { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string Season { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string Duration { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string Distance { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string MaxAltitude { get; set; } = string.Empty;
    
    [StringLength(2000)]
    public string TrekDescription { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string Image { get; set; } = string.Empty;
    
    [StringLength(50)]
    public string AgeGroup { get; set; } = string.Empty;
    
    [StringLength(20)]
    public string GuideNeeded { get; set; } = string.Empty;
    
    [StringLength(10)]
    public string SnowTrek { get; set; } = string.Empty;
    
    [StringLength(1000)]
    public string RecommendedGear { get; set; } = string.Empty;
}