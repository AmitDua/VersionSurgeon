using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using VersionSurgeon.Core.Models;

namespace VersionSurgeon.Core.Services
{
    public class VersionComparer
    {
        private readonly ILogger<VersionComparer> _logger;

        public VersionComparer(ILogger<VersionComparer> logger)
        {
            _logger = logger;
        }

        public ChangeType Compare(List<string> oldMembers, List<string> newMembers)
        {
            try
            {
                _logger.LogInformation("Comparing member surfaces...");

                var removed = oldMembers.Except(newMembers).ToList();
                var added = newMembers.Except(oldMembers).ToList();

                if (removed.Any())
                    return ChangeType.Major;
                if (added.Any())
                    return ChangeType.Minor;

                return ChangeType.None;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error comparing versions.");
                return ChangeType.Major;
            }
        }
    }
}
