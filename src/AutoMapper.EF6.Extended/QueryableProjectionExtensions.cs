using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using DelegateDecompiler.EntityFramework;

namespace AutoMapper.EF6.Extended
{
  public static class QueryableProjectionExtensions
  {
    public static async Task<List<TDestination>> ProjectToListAsync<TDestination>(this IQueryable queryable,
      IConfigurationProvider config, Action<TDestination> postProjectionAction)
    {
      var list = await queryable.ProjectTo<TDestination>(config).DecompileAsync().ToListAsync();
      if (postProjectionAction != null)
      {
        list.ForEach(postProjectionAction);
      }

      return list;
    }

    public static async Task<List<TDestination>> ProjectToListAsync<TDestination>(this IQueryable queryable,
      IConfigurationProvider config, object parameters, Action<TDestination> postProjectionAction)
    {
      var list = await queryable.ProjectTo<TDestination>(config, parameters).DecompileAsync().ToListAsync();
      if (postProjectionAction != null)
      {
        list.ForEach(postProjectionAction);
      }

      return list;
    }
  }
}