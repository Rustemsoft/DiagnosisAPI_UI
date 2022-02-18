using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DDxApi.Models
{
    public class DDxItem
    {
        public string Id { get; set; }
        public string Tests { get; set; }
        public string Symptoms { get; set; }
    }

    [Keyless]
    [NotMapped]
    public class TestType
    {
        public TestType(int id, string procedure, string lowRangeValue, string highRangeValue, string unitsType, string lowRangeValueSI, string highRangeValueSI, string unitsTypeSI, string comboValuesList, string negativeComboValue, string descrLow, string descrHigh, string sid)
        {
            ID = id;
            Procedure = procedure;
            LowRangeValue = lowRangeValue;
            HighRangeValue = highRangeValue;
            UnitsType = unitsType;
            LowRangeValueSI = lowRangeValueSI;
            HighRangeValueSI = highRangeValueSI;
            UnitsTypeSI = unitsTypeSI;
            ComboValuesList = comboValuesList;
            NegativeComboValue = negativeComboValue;
            DescriptionLow = descrLow;
            DescriptionHigh = descrHigh;
            SID = sid;
        }
        public int ID { get; set; }
        public string Procedure { get; set; }
        public string LowRangeValue { get; set; } 
        public string HighRangeValue { get; set; } 
        public string UnitsType { get; set; } 
        public string LowRangeValueSI { get; set; } 
        public string HighRangeValueSI { get; set; } 
        public string UnitsTypeSI { get; set; } 
        public string ComboValuesList { get; set; } 
        public string NegativeComboValue { get; set; } 
        public string DescriptionLow { get; set; } 
        public string DescriptionHigh { get; set; } 
        public string SID { get; set; }
    }

    [Keyless]
    [NotMapped]
    public class TestPostType
    {
        public TestPostType()
        {
        }

        public TestPostType(int id, string value, string lowRangeValue, string highRangeValue, int referenceType)
        {
            ID = id;
            Value = value;
            LowRangeValue = lowRangeValue;
            HighRangeValue = highRangeValue;
            ReferenceType = referenceType;
        }
        public int ID { get; set; }
        public string Value { get; set; }
        public string LowRangeValue { get; set; }
        public string HighRangeValue { get; set; }
        public int ReferenceType { get; set; }
    }

    [Keyless]
    [NotMapped]
    public class TestGetType
    {
        public TestGetType()
        {
        }

        public TestGetType(int id, bool entered, string value, string status, string unitsType, string lowRangeValue, string highRangeValue, string unitsTypeSI, string lowRangeValueSI, string highRangeValueSI, string comboValuesList, string negativeComboValue, string comment, int referenceType)
        {
            ID = id;
            Entered = entered;
            Value = value;
            Status = status;
            UnitsType = unitsType;
            LowRangeValue = lowRangeValue;
            HighRangeValue = highRangeValue;
            UnitsTypeSI = unitsTypeSI;
            LowRangeValueSI = lowRangeValueSI;
            HighRangeValueSI = highRangeValueSI;
            ComboValuesList = comboValuesList;
            NegativeComboValue = negativeComboValue;
            Comment = comment;
            ReferenceType = referenceType;  // 0 - Conventional Units
                                            // 1 - SI Reference Ranges
        }
        public int ID { get; set; }
        public bool Entered { get; set; } 
        public string Value { get; set; }
        public string Status { get; set; } // Low, High, Normal, none 
        public string UnitsType { get; set; }
        public string LowRangeValue { get; set; }
        public string HighRangeValue { get; set; }
        public string UnitsTypeSI { get; set; }
        public string LowRangeValueSI { get; set; }
        public string HighRangeValueSI { get; set; }
        public string ComboValuesList { get; set; } 
        public string NegativeComboValue { get; set; } // if UnitsType is Combo
        public string Comment { get; set; }
        public int ReferenceType { get; set; }
    }

    public class TestsDropdownList
    {
        public string id { get; set; }
        public string procedure { get; set; }
    }

    public class PanelsDropdownList
    {
        public string sid { get; set; }
        public string panel { get; set; }
    }

    [Keyless]
    [NotMapped]
    public class SymptomType
    {
        public SymptomType(int id, string symptom, string similarSymptomsList, string category)
        {
            ID = id;
            Symptom = symptom;
            SimilarSymptomsList = similarSymptomsList;
            Category = category;
        }
        public int ID { get; set; }
        public string Symptom { get; set; }
        public string SimilarSymptomsList { get; set; }
        public string Category { get; set; }
    }

    [Keyless]
    [NotMapped]
    public class SymptomPostType
    {
        public SymptomPostType(int id)
        {
            ID = id;
        }
        public int ID { get; set; }
    }

    [Keyless]
    [NotMapped]
    public class SymptomGetType
    {
        public SymptomGetType()
        {
        }

        public SymptomGetType(int id, string symptom, bool entered, string similarSymptomsList, string category)
        {
            ID = id;
            Symptom = symptom;
            Entered = entered;
            SimilarSymptomsList = similarSymptomsList;
            Category = category;
        }
        public int ID { get; set; }
        public string Symptom { get; set; }
        public bool Entered { get; set; } 
        public string SimilarSymptomsList { get; set; }
        public string Category { get; set; }
    }

    [Keyless]
    [NotMapped]
    public class TestsSymptomsModel
    {
        public ICollection<TestPostType> Tests { get; set; }
        public ICollection<SymptomPostType> Symptoms { get; set; }
    }

    [Keyless]
    [NotMapped]
    public class DisorderTestsSymptomsModel
    {
        public Disorder disorder = new Disorder();
        public ICollection<TestGetType> Tests { get; set; }
        public ICollection<SymptomGetType> Symptoms { get; set; }
    }

    public class SymptomsDropdownList
    {
        public string id { get; set; }
        public string symptom { get; set; }
    }

    public class SymptomCategoriesDropdownList
    {
        public string sid { get; set; }
        public string category { get; set; }
    }

    public class Disorder
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ICD9 { get; set; }
        public string ICD10 { get; set; }
        public string ICD11 { get; set; }
        public int Weight { get; set; }
        public string Tests { get; set; }
        public string Symptoms { get; set; }
    }
}
