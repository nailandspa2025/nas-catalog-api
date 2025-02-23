using System.Linq.Expressions;
using System.Reflection;
using BuildingBlocks.Authentication.Abstractions;
using BuildingBlocks.MultiTenancy.Abstractions;
using BuildingBlocks.Persistence.Abstractions.Entities;
using BuildingBlocks.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BuildingBlocks.Persistence.EntityFrameworkCore;

public abstract class EfCoreDbContext<TDbContext> : DbContext, IEfCoreDbContext where TDbContext : DbContext
{
    private readonly ITenantResolver _tenantResolver;
    private readonly ICurrentUser _currentUser;

    public EfCoreDbContext(
        DbContextOptions<TDbContext> options,
        ICurrentUser currentUser,
        ITenantResolver tenantResolver)
        : base(options)
    {
        _tenantResolver = tenantResolver;
        _currentUser = currentUser;
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

    protected virtual void ConfigureBaseProperties<TEntity>(ModelBuilder modelBuilder, IMutableEntityType mutableEntityType)
        where TEntity : class
    {
        if (mutableEntityType.IsOwned())
        {
            return;
        }

        modelBuilder.Entity<TEntity>().ConfigureByConvention();
        ConfigureGlobalFilters<TEntity>(modelBuilder, mutableEntityType);
    }

    protected virtual void ConfigureGlobalFilters<TEntity>(ModelBuilder modelBuilder, IMutableEntityType mutableEntityType)
        where TEntity : class
    {
        if (mutableEntityType.BaseType == null && ShouldFilterEntity<TEntity>(mutableEntityType))
        {
            var filterExpression = CreateFilterExpression<TEntity>();
            if (filterExpression != null)
            {
                modelBuilder.Entity<TEntity>().HasQueryFilter(filterExpression);
            }
        }
    }

    protected virtual Expression<Func<TEntity, bool>> CreateFilterExpression<TEntity>()
        where TEntity : class
    {
        Expression<Func<TEntity, bool>> expression = null;

        if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
        {
            expression = e => !EF.Property<bool>(e, nameof(ISoftDelete.IsDeleted));
        }

        if (typeof(IMultiTenant).IsAssignableFrom(typeof(TEntity)))
        {
            var tenant = _tenantResolver.GetCurrentTenantConfiguration();
            Expression<Func<TEntity, bool>> multiTenantFilter = e => EF.Property<Guid>(e, nameof(IMultiTenant.TenantId)) == tenant.Id;
            expression = expression == null ? multiTenantFilter : CombineExpressions(expression, multiTenantFilter);
        }

        return expression;
    }

    protected virtual bool ShouldFilterEntity<TEntity>(IMutableEntityType entityType) where TEntity : class
    {
        if (typeof(IMultiTenant).IsAssignableFrom(typeof(TEntity)))
        {
            return true;
        }

        if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
        {
            return true;
        }

        return false;
    }

    protected virtual Expression<Func<T, bool>> CombineExpressions<T>(Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
    {
        var parameter = Expression.Parameter(typeof(T));

        var leftVisitor = new ReplaceExpressionVisitor(expression1.Parameters[0], parameter);
        var left = leftVisitor.Visit(expression1.Body);

        var rightVisitor = new ReplaceExpressionVisitor(expression2.Parameters[0], parameter);
        var right = rightVisitor.Visit(expression2.Body);

        return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left, right), parameter);
    }

    class ReplaceExpressionVisitor : ExpressionVisitor
    {
        private readonly Expression _oldValue;
        private readonly Expression _newValue;

        public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public override Expression Visit(Expression node)
        {
            if (node == _oldValue)
            {
                return _newValue;
            }

            return base.Visit(node);
        }
    }
    private static readonly MethodInfo ConfigureBasePropertiesMethodInfo
        = typeof(EfCoreDbContext<TDbContext>)
            .GetMethod(
                nameof(ConfigureBaseProperties),
                BindingFlags.Instance | BindingFlags.NonPublic
            );
}
