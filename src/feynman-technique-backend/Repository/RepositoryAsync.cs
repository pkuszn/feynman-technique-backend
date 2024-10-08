﻿using FeynmanTechniqueBackend.Models;
using FeynmanTechniqueBackend.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace FeynmanTechniqueBackend.Repository
{
    public class RepositoryAsync : IRepositoryAsync
    {
        private readonly FeynmanTechniqueCorpusContext DbContext;
        public RepositoryAsync(FeynmanTechniqueCorpusContext dbContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<List<E>> GetWhereAsync<E>(Expression<Func<E, bool>> expression, CancellationToken cancellationToken)
            where E : class
        {
            return await DbContext.Set<E>().Where(expression).ToListAsync(cancellationToken: cancellationToken);
        }

        public async Task<List<E>> GetAllAsync<E>(CancellationToken cancellationToken)
            where E : class
        {
            return await DbContext.Set<E>().ToListAsync(cancellationToken: cancellationToken);
        }

        public async Task<E> GetByIdAsync<E, T>(T id, CancellationToken cancellationToken)
            where E : class, IEntity<T>
        {
            E? entity = await DbContext.Set<E>().FirstOrDefaultAsync(f => f.Id.Equals(id), cancellationToken: cancellationToken);
            return entity;
        }

        public async Task<E> PostAsync<E>(E entity, CancellationToken cancellationToken)
            where E : class
        {
            DbContext.Set<E>().Add(entity);
            await DbContext.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task<E> PutAsync<E, T>(E entity, CancellationToken cancellationToken)
            where E : class, IEntity<T>
        {
            E? foundEntity = await DbContext.Set<E>().FirstOrDefaultAsync(f => f.Id.Equals(entity.Id), cancellationToken: cancellationToken);
            if (foundEntity == null)
            {
                return null;
            }

            foundEntity = entity;
            await DbContext.SaveChangesAsync(cancellationToken);
            return foundEntity;
        }

        public async Task<bool> DeleteAsync<E, T>([FromRoute]T id, CancellationToken cancellationToken)
            where E : class, IEntity<T>
        {
            E? entity = await DbContext.Set<E>().FirstOrDefaultAsync(f => f.Id.Equals(id), cancellationToken: cancellationToken);
            if (entity == null)
            {
                return false;
            }

            DbContext.Set<E>().Remove(entity);
            await DbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<List<E>> BulkInsertAsync<E>(IEnumerable<E> entities, CancellationToken cancellationToken)
            where E : class
        {
            await DbContext.Set<E>().AddRangeAsync(entities, cancellationToken);
            await DbContext.SaveChangesAsync(cancellationToken);
            return new List<E>(entities);
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
        {
            return await DbContext.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task<List<E>> GetWhereLimitAsync<E>(Expression<Func<E, bool>> expression, int offset, int partOfSet, CancellationToken cancellationToken) where E : class
        {
            int skip = partOfSet > 1 ? partOfSet * offset : 0;
            return await DbContext
                .Set<E>()
                .Where(expression)
                .Skip(skip)
                .Take(offset)
                .ToListAsync(cancellationToken: cancellationToken);
        }

        public async Task<int> GetAmountOfEntriesAsync<E>(CancellationToken cancellationToken) where E: class
        {
            return await DbContext
                .Set<E>()
                .CountAsync(cancellationToken: cancellationToken);
        }

        public IProperty? TryGetColumnName<E>(string columnName) where E : class
        {
            IEntityType? type = DbContext.Model.FindEntityType(typeof(E));
            if (type == null)
            {
                return null;
            }

            return type.FindProperty(columnName);
        }

        public async Task<List<object>> GetByColumnAsync<E>(IProperty property, CancellationToken cancellationToken) 
            where E : class
        {
            //TODO: Dodać WHERE, gdzie filtrujemy po zadanej kolumnie
            return await DbContext.Set<E>()
                .Select(CreateSelector<E>(property.Name))
                .ToListAsync(cancellationToken);
        }

        private static Expression<Func<E, object>> CreateSelector<E>(string propertyName) where E : class
        {
            ParameterExpression parameter = Expression.Parameter(typeof(E), "x");
            MemberExpression property = Expression.Property(parameter, propertyName);
            UnaryExpression conversion = Expression.Convert(property, typeof(object));
            return Expression.Lambda<Func<E, object>>(conversion, parameter);
        }
    }
}
