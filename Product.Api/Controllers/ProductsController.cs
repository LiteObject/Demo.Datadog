using Datadog.Trace;
using Microsoft.AspNetCore.Mvc;
using Product.Api.Services;

namespace Product.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly IProductService _productService;

        public ProductsController(ILogger<ProductsController> logger, IProductService productService)
        {
            _logger = logger;
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));

            _logger.LogInf($"Instantiated {nameof(ProductsController)} class.");
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            _logger.LogInf($"Invoked {nameof(Get)} method.");

            // Access the active scope through the global tracer (can return null)
            IScope scope = Tracer.Instance.ActiveScope;

            // Add a tag to the span for use in the datadog web UI
            _ = (scope?.Span.SetTag("customer.id", "mhoque@email.io"));

            List<Entities.Product> products = await _productService.FindAsync(p => p.IsAvailable);
            return products is null || !products.Any() ?
                NotFound() :
                Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInf($"Invoked {nameof(GetById)} method.");

            if (id == default)
            {
                return BadRequest();
            }

            // Entities.Product? Product = await Task.FromResult(StaticProductService.GetById(id));
            Entities.Product? product = await _productService.GetByIdAsync(id);

            return product == null ?
                NotFound($"No record with id {id} found in the system.") :
                Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Entities.Product product)
        {
            _logger.LogInf($"Invoked {nameof(Post)} method.");

            // We can also call ModelState validation globally
            if (!ModelState.IsValid)
            {
                _logger.LogWrn($"Invalid model state: {ModelState}");
                return BadRequest(ModelState);
            }

            await Task.Delay(1);
            _logger.LogInf("Adding a new product");
            return Created(new Uri("/"), product);
        }

        [HttpGet("ThrowException")]
        public IActionResult Throw()
        {
            throw new ApplicationException("This is a test. Please ignore.");
        }

        /// <summary>
        /// Returns an RFC-7807 compliant payload to the client.
        /// https://www.rfc-editor.org/rfc/rfc7807
        /// </summary>
        /// <returns></returns>
        /*[Route("/error")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult HandleError()
        {
            //{
            //    "type": "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            //    "title": "An error occurred while processing your request.",
            //    "status": 500,
            //    "traceId": "00-5d88da43d3bd1d024f7fb7429794daf6-f904798140f7f589-00"
            //}

            return Problem();
        } */

        // <summary>
        /// Returns an RFC-7807 compliant payload to the client.
        /// https://www.rfc-editor.org/rfc/rfc7807
        /// </summary>
        /// <returns></returns>
        /*[Route("/error-details")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult HandleError([FromServices] IHostEnvironment hostEnvironment)
        {
            //{
            //    "type": "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            //    "title": "An error occurred while processing your request.",
            //    "status": 500,
            //    "traceId": "00-5d88da43d3bd1d024f7fb7429794daf6-f904798140f7f589-00"
            //}

            IExceptionHandlerFeature exceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>()!;

            _logger.LogErr(exceptionHandlerFeature.Error.Message, exceptionHandlerFeature.Error);

            return hostEnvironment.IsProduction()
                ? Problem(
                detail: exceptionHandlerFeature.Error.Message,
                title: "We have encountered an unexpected event.")
                : (IActionResult)Problem(
                detail: exceptionHandlerFeature.Error.StackTrace,
                title: exceptionHandlerFeature.Error.Message);
        } */

        /*[HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Entities.Product Product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != Product?.Id)
            {
                return BadRequest();
            }

            // Faking async call :(
            Entities.Product? existingProduct = await Task.FromResult(StaticProductService.GetById(id));

            if (existingProduct is null)
            {
                return NotFound($"No record with id {id} found in the system.");
            }

            StaticProductService.Update(Product);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            Entities.Product? existingProduct = StaticProductService.GetById(id);

            if (existingProduct is null)
            {
                return NotFound();
            }

            // Delete Product here
            StaticProductService.Delete(id);

            return NoContent();
        } */
    }
}