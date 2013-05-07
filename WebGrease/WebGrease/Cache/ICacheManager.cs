// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICacheManager.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WebGrease
{
    using System;
    using System.IO;

    /// <summary>The CacheManager interface.</summary>
    public interface ICacheManager
    {
        #region Public Properties

        /// <summary>Gets the current cache section.</summary>
        ICacheSection CurrentCacheSection { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Begins a new cache section.</summary>
        /// <param name="category">The category.</param>
        /// <param name="settings">The settings.</param>
        /// <returns>The <see cref="ICacheSection"/>.</returns>
        ICacheSection BeginSection(string category, object settings);

        /// <summary>Begins a new cache section.</summary>
        /// <param name="category">The category.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="settings">The settings.</param>
        /// <returns>The <see cref="ICacheSection"/>.</returns>
        ICacheSection BeginSection(string category, FileInfo filePath, object settings = null);

        /// <summary>Cleans up all the cache files that we don't need anymore.</summary>
        void CleanUp();

        /// <summary>Ends the cache section.</summary>
        /// <param name="cacheSection">The cache section.</param>
        void EndSection(ICacheSection cacheSection);

        /// <summary>Gets absolute cache file path.</summary>
        /// <param name="category">The category.</param>
        /// <param name="fileName">The relative cache file name.</param>
        /// <returns>The absolute cache file path.</returns>
        string GetAbsoluteCacheFilePath(string category, string fileName);

        /// <summary>Loads a cache section from disk, uses per session in memory cache as well.</summary>
        /// <param name="fullPath">The full path.</param>
        /// <param name="loadAction">The load action.</param>
        /// <typeparam name="T">The Type of ICacheSection</typeparam>
        /// <returns>The cache section.</returns>
        T LoadCacheSection<T>(string fullPath, Func<T> loadAction) where T : class, ICacheSection;

        /// <summary>Sets the current context.</summary>
        /// <param name="newContext">The current context.</param>
        void SetContext(IWebGreaseContext newContext);

        /// <summary>Stores the content in cache.</summary>
        /// <param name="category">The category.</param>
        /// <param name="content">The content.</param>
        /// <returns>The stored cache file path.</returns>
        string StoreContentInCache(string category, string content);

        /// <summary>Stores the file in cache.</summary>
        /// <param name="category">The category.</param>
        /// <param name="absolutePath">The absolute path.</param>
        /// <returns>The stored cache file path.</returns>
        string StoreFileInCache(string category, string absolutePath);

        #endregion
    }
}