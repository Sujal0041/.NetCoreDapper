using System.ComponentModel.DataAnnotations;

public class Branch
{

    public int Id { get; set; }
    public bool IsCorporate { get; set; }

    [StringLength(50)]
    public required string ParentId { get; set; }

    [StringLength(100)]
    public required string Alias { get; set; }

    [StringLength(100)]
    public required string FullName { get; set; }

    [StringLength(100)]
    public required string PuraName { get; set; }

    [StringLength(50)]
    public required string ShortName { get; set; }

    [StringLength(200)]
    public required string StreetName { get; set; }

    [StringLength(200)]
    public required string StreetNameLocale { get; set; }

    public int? WardNumber { get; set; }

    [StringLength(100)]
    public required string WardNumberLocale { get; set; }

    [StringLength(100)]
    public required string LocalMNC { get; set; }

    [StringLength(100)]
    public required string LocalMNC_Locale { get; set; }

    [StringLength(100)]
    public required string City { get; set; }

    [StringLength(100)]
    public required string City_Locale { get; set; }

    [StringLength(100)]
    public required string District { get; set; }

    [StringLength(100)]
    public required string District_Locale { get; set; }
}
