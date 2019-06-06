using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SphinxConnectorSample.Model.Parameters;
using SphinxConnectorSample.Model.Sphinx;
using SphinxConnectorSample.Services;

namespace SphinxConnectorSample.Controllers
{
    [Produces("application/json")]
    [Route("listings")]
    [ApiController]
    public class ListingsController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly ISphinxService _sphinx;

        public ListingsController(ISphinxService sphinx, ILogger<ListingsController> logger)
        {
            _sphinx = sphinx;
            _logger = logger;
        }

        [HttpGet]
        [Route("search")]
        [ProducesResponseType(typeof(List<SphinxConnectorTest>), 200)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(500)]
        public IActionResult Search([FromQuery] SearchParameters searchParameters)
        {
            
            try
            {
                var searchCriteria = new SearchCriteria(searchParameters);
                TryValidateModel(searchCriteria);
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);


                var searchResults = _sphinx.Search(searchCriteria);
                return Ok(searchResults);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);

                return StatusCode(500, "An unexpected error has occurred.");
            }
        }

        /// <summary>
        /// Returns the current deployed version of the search assembly
        /// </summary>
        /// <returns>a string with the deployed version number</returns>
        [HttpGet]
        [Route("search/version")]
        [ProducesResponseType(typeof(string), 200)]
        public string GetVersion()
        {
            _logger.LogInformation("Get version");

            return $"Version {Assembly.GetExecutingAssembly().GetName().Version}";
        }
    }
}
