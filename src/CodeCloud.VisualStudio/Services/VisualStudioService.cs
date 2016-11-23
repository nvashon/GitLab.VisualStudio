﻿using CodeCloud.VisualStudio.Shared;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TeamFoundation.Git.Extensibility;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace CodeCloud.VisualStudio.Services
{
    [Export(typeof(IVisualStudioService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class VisualStudioService : IVisualStudioService
    {
        public int IGitExt { get; private set; }

        public IReadOnlyList<Project> Projects { get; set; }

        public IEnumerable<Repository> Repositories { get; set; }

        public IServiceProvider ServiceProvider { get; set; }

        public RepositoryInfo GetActiveRepository()
        {
            if (ServiceProvider == null)
            {
                return null;
            }

            var git = ServiceProvider.GetService<IGitExt>();
            var repo = git.ActiveRepositories.FirstOrDefault();

            if (repo != null)
            {
                return new RepositoryInfo
                {
                    Path = repo.RepositoryPath,
                    Branch = repo.CurrentBranch.Name
                };
            }
            return null;
        }

        public string GetSolutionPath()
        {
            if (ServiceProvider == null)
            {
                return null;
            }
            var solution = (IVsSolution)ServiceProvider.GetService(typeof(SVsSolution));

            string solutionDir, solutionFile, userFile;
            if (!ErrorHandler.Succeeded(solution.GetSolutionInfo(out solutionDir, out solutionFile, out userFile)))
            {
                return null;
            }

            if (solutionDir == null)
            {
                return null;
            }

            return solutionDir;
        }
    }
}
