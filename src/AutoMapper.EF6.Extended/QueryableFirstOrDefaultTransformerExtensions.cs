using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using DelegateDecompiler;
using DelegateDecompiler.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AutoMapper.EF6.Extended
{
  public static class QueryableFirstOrDefaultTransformerExtensions
  {
    /// <summary>
    ///   Returns the first transformed (calling Transform of <see cref="IPostProjectionTransformer" />) element of a sequence
    ///   asynchronously, or a default value if the sequence contains no elements.
    /// </summary>
    /// <typeparam name="TDestination">The type of the destination.</typeparam>
    /// <param name="queryable">The queryable.</param>
    /// <param name="config">The configuration.</param>
    /// <returns>
    ///   A task that represents the asynchronous operation.
    ///   The task result contains <c>default</c> ( <typeparamref name="TDestination" /> ) if
    ///   <paramref name="queryable" /> is empty; otherwise, the first element in <paramref name="queryable" />.
    /// </returns>
    public static Task<TDestination> ProjectToFirstOrDefaultTransformAsync<TDestination>(
      this IQueryable queryable, IConfigurationProvider config)
      where TDestination : IPostProjectionTransformer
    {
      return ProjectToFirstOrDefaultTransformAsync<TDestination>(queryable, config, null, CancellationToken.None);
    }

    /// <summary>
    ///   Returns the first transformed (calling Transform of <see cref="IPostProjectionTransformer" />) element of a sequence
    ///   asynchronously, or a default value if the sequence contains no elements.
    /// </summary>
    /// <typeparam name="TDestination">The type of the destination.</typeparam>
    /// <param name="queryable">The queryable.</param>
    /// <param name="config">The configuration.</param>
    /// <param name="parameters">The values to be used for parameterization.</param>
    /// <returns>
    ///   A task that represents the asynchronous operation.
    ///   The task result contains <c>default</c> ( <typeparamref name="TDestination" /> ) if
    ///   <paramref name="queryable" /> is empty; otherwise, the first element in <paramref name="queryable" />.
    /// </returns>
    public static Task<TDestination> ProjectToFirstOrDefaultTransformAsync<TDestination>(
      this IQueryable queryable, IConfigurationProvider config, object parameters)
      where TDestination : IPostProjectionTransformer
    {
      return ProjectToFirstOrDefaultTransformAsync<TDestination>(queryable, config, parameters, CancellationToken.None);
    }

    /// <summary>
    ///   Returns the first transformed (calling Transform of <see cref="IPostProjectionTransformer" />) element of a sequence
    ///   asynchronously, or a default value if the sequence contains no elements.
    /// </summary>
    /// <typeparam name="TDestination">The type of the destination.</typeparam>
    /// <param name="queryable">The queryable.</param>
    /// <param name="config">The configuration.</param>
    /// <param name="cancellationToken">
    ///   A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the
    ///   task to complete.
    /// </param>
    /// <returns>
    ///   A task that represents the asynchronous operation.
    ///   The task result contains <c>default</c> ( <typeparamref name="TDestination" /> ) if
    ///   <paramref name="queryable" /> is empty; otherwise, the first element in <paramref name="queryable" />.
    /// </returns>
    /// <exception cref="System.OperationCanceledException">
    ///   Thrown if
    ///   <param name="cancellationToken">cancellationToken</param>
    ///   requests cancellation.
    /// </exception>
    public static Task<TDestination> ProjectToFirstOrDefaultTransformAsync<TDestination>(
      this IQueryable queryable, IConfigurationProvider config,
      CancellationToken cancellationToken)
      where TDestination : IPostProjectionTransformer
    {
      return ProjectToFirstOrDefaultTransformAsync<TDestination>(queryable, config, null, cancellationToken);
    }

    /// <summary>
    ///   Returns the first transformed (calling Transform of <see cref="IPostProjectionTransformer" />) element of a sequence
    ///   asynchronously, or a default value if the sequence contains no elements.
    /// </summary>
    /// <typeparam name="TDestination">The type of the destination.</typeparam>
    /// <param name="queryable">The queryable.</param>
    /// <param name="config">The configuration.</param>
    /// <param name="parameters">The values to be used for parameterization.</param>
    /// <param name="cancellationToken">
    ///   A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the
    ///   task to complete.
    /// </param>
    /// <returns>
    ///   A task that represents the asynchronous operation.
    ///   The task result contains <c>default</c> ( <typeparamref name="TDestination" /> ) if
    ///   <paramref name="queryable" /> is empty; otherwise, the first element in <paramref name="queryable" />.
    /// </returns>
    /// <exception cref="System.OperationCanceledException">
    ///   Thrown if
    ///   <param name="cancellationToken">cancellationToken</param>
    ///   requests cancellation.
    /// </exception>
    public static async Task<TDestination> ProjectToFirstOrDefaultTransformAsync<TDestination>(
      this IQueryable queryable, IConfigurationProvider config, object parameters,
      CancellationToken cancellationToken)
      where TDestination : IPostProjectionTransformer
    {
      var item =
        await
          queryable.ProjectTo<TDestination>(config, parameters).DecompileAsync().FirstOrDefaultAsync(cancellationToken);

      if (item != null)
      {
        cancellationToken.ThrowIfCancellationRequested();

        item.Transform();
      }

      return item;
    }

    /// <summary>
    ///   Returns the first transformed (calling Transform of <see cref="IPostProjectionTransformer" />) element of a sequence,
    ///   or a default value if the sequence contains no elements.
    /// </summary>
    /// <typeparam name="TDestination">The type of the destination.</typeparam>
    /// <param name="queryable">The queryable.</param>
    /// <param name="config">The configuration.</param>
    /// <param name="parameters">The values to be used for parameterization.</param>
    /// <returns>
    ///   A task that represents the asynchronous operation.
    ///   The task result contains <c>default</c> ( <typeparamref name="TDestination" /> ) if
    ///   <paramref name="queryable" /> is empty; otherwise, the first element in <paramref name="queryable" />.
    /// </returns>
    public static TDestination ProjectToFirstOrDefaultTransform<TDestination>(
      this IQueryable queryable, IConfigurationProvider config, object parameters)
      where TDestination : IPostProjectionTransformer
    {
      var item =
        queryable.ProjectTo<TDestination>(config, parameters).Decompile().FirstOrDefault();

      if (item != null)
      {
        item.Transform();
      }

      return item;
    }
  }
}