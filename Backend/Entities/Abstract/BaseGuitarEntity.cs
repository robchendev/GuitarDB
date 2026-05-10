using Backend.Entities.Catalog;

namespace Backend.Entities.Abstract;

public abstract class BaseGuitarEntity
{
  // SHARED META INFO 
  public string Name { get; set; } = string.Empty;
  public string? Description { get; set; }
  // public int? Year { get; set; } // replace with Year? Month? Day? To separate unknowns

  // ORIGIN INFO
  public int? LuthierId { get; set; }
  public Luthier? Luthier { get; set; }
  public int? BrandId { get; set; }
  public Brand? Brand { get; set; }
  public string? CountryId { get; set; } // jp, ca, us
  public Country? Country { get; set; }

  // SHARED CONSTRUCTION INFO
  public int? GuitarTypeId { get; set; } // Acoustic, Electric, Classical
  public GuitarType? GuitarType { get; set; }
  public int? GuitarBodyShapeId { get; set; }
  public GuitarBodyShape? GuitarBodyShape { get; set; }
  public int? GuitarBracingTypeId { get; set; }
  public GuitarBracingType? GuitarBracingType { get; set; }
  public int? NumberOfStrings { get; set; }
  public int? NumberOfFrets { get; set; }
  public decimal? ScaleLength { get; set; } // e.g. 650mm,
  public bool? IsLeftHanded { get; set; }
  public int? WeightGrams { get; set; }

  // Materials: If MaterialConstruction or MaterialGrade exists, then Material must exist.
  // Enforce this programmatically and on a DB level

  // SHARED MATERIAL INFO 

  // FINGERBOARD
  public int? FingerboardMaterialId { get; set; }
  public Material? FingerboardMaterial { get; set; }
  public int? FingerboardMaterialConstructionId { get; set; }
  public MaterialConstruction? FingerboardMaterialConstruction { get; set; }
  public int? FingerboardMaterialGradeId { get; set; }
  public MaterialGrade? FingerboardMaterialGrade { get; set; }

  // NECK
  public int? NeckMaterialId { get; set; }
  public Material? NeckMaterial { get; set; }
  public int? NeckMaterialConstructionId { get; set; }
  public MaterialConstruction? NeckMaterialConstruction { get; set; }
  public int? NeckMaterialGradeId { get; set; }
  public MaterialGrade? NeckMaterialGrade { get; set; }

  // ACOUSTIC MATERIAL INFO

  // TOP
  public int? TopMaterialId { get; set; }
  public Material? TopMaterial { get; set; }
  public int? TopMaterialConstructionId { get; set; }
  public MaterialConstruction? TopMaterialConstruction { get; set; }
  public int? TopMaterialGradeId { get; set; }
  public MaterialGrade? TopMaterialGrade { get; set; }

  // BACK
  public int? BackMaterialId { get; set; }
  public Material? BackMaterial { get; set; }
  public int? BackMaterialConstructionId { get; set; }
  public MaterialConstruction? BackMaterialConstruction { get; set; }
  public int? BackMaterialGradeId { get; set; }
  public MaterialGrade? BackMaterialGrade { get; set; }

  // SIDES
  public int? SidesMaterialId { get; set; }
  public Material? SidesMaterial { get; set; }
  public int? SidesMaterialConstructionId { get; set; }
  public MaterialConstruction? SidesMaterialConstruction { get; set; }
  public int? SidesMaterialGradeId { get; set; }
  public MaterialGrade? SidesMaterialGrade { get; set; }

  // BRIDGE
  public int? BridgeMaterialId { get; set; }
  public Material? BridgeMaterial { get; set; }
  public int? BridgeMaterialConstructionId { get; set; }
  public MaterialConstruction? BridgeMaterialConstruction { get; set; }
  public int? BridgeMaterialGradeId { get; set; }
  public MaterialGrade? BridgeMaterialGrade { get; set; }

  // BINDING
  public int? BindingMaterialId { get; set; }
  public Material? BindingMaterial { get; set; }

  // SADDLE
  public int? SaddleMaterialId { get; set; }
  public Material? SaddleMaterial { get; set; }

  // NUT
  public int? NutMaterialId { get; set; }
  public Material? NutMaterial { get; set; }

  // BRIDGE PIN
  public int? BridgePinMaterialId { get; set; }
  public Material? BridgePinMaterial { get; set; }

  // If the guitar type is electric, we add more columns here. It would share some properties above like weight, frets, strings
  // but the fields like TopMaterial would be null. Null columns take up a trivial amount of space.
}
