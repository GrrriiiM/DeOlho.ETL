using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DeOlho.ETL.Transforms
{
    public class DescompressStreamTransform : StepTransform<Stream, IEnumerable<StreamDescompressed>>
    {

        public DescompressStreamTransform(IStep<Stream> stepIn)
            : base(stepIn, null)
        {
        }

        public async override Task<StepValue<IEnumerable<StreamDescompressed>>> Execute()
        {
            var @in = await this._stepIn.Execute();
            return @in.DescompressStream();
        }
    }

    
    public static class DescompressStreamTransformExtensions
    {
        public static IStep<IEnumerable<StreamDescompressed>> TransformDescompressStream(this IStep<Stream> value)
        {
            return new DescompressStreamTransform(value);
        }

        public static IEnumerable<StepValue<IEnumerable<StreamDescompressed>>> TransformDescompressStream(this IEnumerable<StepValue<Stream>> value)
        {
            return Enumerable.Select(value, _ => DescompressStream(_));
        }

        public static StepValue<IEnumerable<StreamDescompressed>> DescompressStream(this StepValue<Stream> value)
        {
            var zipArchive = new ZipArchive(value.Value);
            var streams = new List<StreamDescompressed>();


            foreach(var entry in zipArchive.Entries)
            {
                streams.Add(new StreamDescompressed
                {
                    Name = entry.Name,
                    Stream = entry.Open()
                });
            }

            return new StepValue<IEnumerable<StreamDescompressed>>(streams, value);
        }
    }

    public class StreamDescompressed
    {
        public string Name { get; set; }
        public Stream Stream { get; set; }
        
    }
}