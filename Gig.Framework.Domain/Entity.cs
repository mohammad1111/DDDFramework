using System;
using System.ComponentModel.DataAnnotations.Schema;
using Gig.Framework.Core.Enums;
using Gig.Framework.Domain.IdGenerators;

namespace Gig.Framework.Domain;
//todo:Check inject IServiceLocator for IRequestContext

public abstract class Entity : IEquatable<Entity>
{
    private readonly IGigIdGenerator _idGenerator;

    protected Entity()
    {
    }

    protected Entity(IGigIdGenerator idGenerator)
    {
        _idGenerator = idGenerator;
        Id = idGenerator.NewId();
        RecGuid = Guid.NewGuid();
        CreatedOn = DateTime.Now;
        ModifiedOn = DateTime.Now;
        IsDeleted = false;
        StateCode = StateCodeEnum.ActiveCode;
        IsInUpdateMode = false;
    }

    [NotMapped]
    public bool IsInUpdateMode { get; private set; } = true;
    
    public long Id { get; protected set; }
    public Guid RecGuid { get; protected set; }
    public long CompanyId { get; set; }
    public long BranchId { get; set; }
    public long OwnerId { get; set; }
    public long CreatedBy { get; set; }
    public DateTime CreatedOn { get; protected set; }
    public long ModifiedBy { get; set; }
    public DateTime ModifiedOn { get; set; }
    public StateCodeEnum StateCode { get; protected set; }
    public bool IsDeleted { get; protected set; }
    public virtual byte[] RowVersion { get; protected set; }
    
    public virtual bool Equals(Entity other)
    {
        return RecGuid.Equals(other.RecGuid);
    }

    public virtual void Active()
    {
        StateCode = StateCodeEnum.ActiveCode;
    }

    public virtual void Pend()
    {
        StateCode = StateCodeEnum.Pend;
    }

    public virtual void Lock()
    {
        StateCode = StateCodeEnum.Lock;
    }

    public virtual void DeActive()
    {
        StateCode = StateCodeEnum.DeActive;
    }

    

    public override bool Equals(object obj)
    {
        return base.Equals((Entity)obj);
    }

    public override int GetHashCode()
    {
        return RecGuid.GetHashCode();
    }

    public virtual void SoftDelete()
    {
        IsDeleted = true;
    }
}