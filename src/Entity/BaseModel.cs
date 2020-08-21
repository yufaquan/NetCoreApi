using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entity
{
    public class BaseModel
    {
        /// <summary>
        /// 数据Id
        /// </summary>
        [SugarColumn(ColumnName = "Id")]
        public virtual int Id { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        [SugarColumn(ColumnName = "CreatedBy")]
        public virtual int? CreatedBy { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        [SugarColumn(ColumnName = "ModifiedBy")]
        public virtual int? ModifiedBy { get; set; }

        /// <summary>
        /// 删除人
        /// </summary>
        [SugarColumn(ColumnName = "DeletedBy")]
        public virtual int? DeletedBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnName = "CreatedTime")]
        public virtual DateTime? CreatedAt { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [SugarColumn(ColumnName = "ModifiedTime")]
        public virtual DateTime? ModifiedAt { get; set; }

        /// <summary>
        /// 删除时间
        /// </summary>
        [SugarColumn(ColumnName = "DeletedTime")]
        public virtual DateTime? DeletedAt { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        [SugarColumn(ColumnName = "IsDeleted")]
        public virtual bool IsDeleted { get; set; }
    }
}
