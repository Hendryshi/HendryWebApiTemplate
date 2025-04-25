namespace Common.Domain.Common
{
    public abstract class BaseAuditableEntity : BaseEntity
    {
        public new Guid Id
        {
            get => base.Id;
            set => base.Id = value;
        }

        #region methods
        #region created
        public void SetCreated(DateTime date)
        {
            SysCreated = date;
        }
        #endregion
        #region modified
        private bool _modified = false;
        protected void SetToModify() => _modified = true;
        public bool Modified()
        {
            var result = true && _modified;
            _modified = false;
            return result;
        }

        public void SetModified(DateTime date)
        {
            SetToModify();
            SysLastModified = date;
        }
        #endregion
        #endregion
        public DateTime? SysCreated { get; protected set; }

        public DateTime? SysLastModified { get; protected set; }
    }

}
