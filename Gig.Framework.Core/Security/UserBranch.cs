namespace Gig.Framework.Core.Security;

public class UserBranch
{
    public UserBranch(long id, long parentId, string name, string parentIdName)
    {
        Id = id;
        ParentId = parentId;
        Name = name;
        ParentIdName = parentIdName;
    }


    public long Id { get; }
    public long ParentId { get; }
    public string Name { get; }
    public string ParentIdName { get; }
    public string FullName => $"{ParentIdName} - {Name}";
}