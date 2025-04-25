using NJsonSchema;
using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace HendryTemplate.Api.Swagger
{
    public class AddHeadersOperationProcessor : IOperationProcessor
    {
        public AddHeadersOperationProcessor()
        {
        }

        public bool Process(OperationProcessorContext context)
        {
            foreach (var header in SwaggerInfos.ServiceHeaders)
            {
                context.OperationDescription.Operation.Parameters.Add(new NSwag.OpenApiParameter
                {
                    Name = header,
                    Kind = OpenApiParameterKind.Header,
                    IsRequired = false,
                    Schema = new JsonSchema { Type = JsonObjectType.String }
                });
            }

            return true;
        }

    }
}
