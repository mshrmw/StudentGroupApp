//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace StudentGroup
{
    using System;
    using System.Collections.Generic;
    
    public partial class GroupSubjectTeacher
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public GroupSubjectTeacher()
        {
            this.Grades = new HashSet<Grades>();
        }
    
        public int GroupSubjectTeacherID { get; set; }
        public int GroupID { get; set; }
        public int SubjectID { get; set; }
        public int TeacherID { get; set; }
        public Nullable<int> HoursPerWeek { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Grades> Grades { get; set; }
        public virtual Groups Groups { get; set; }
        public virtual Subjects Subjects { get; set; }
        public virtual Teachers Teachers { get; set; }
    }
}
