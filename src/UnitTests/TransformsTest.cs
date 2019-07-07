using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using DeOlho.ETL;
using DeOlho.ETL.Transforms;
using FluentAssertions;
using Moq;
using Xunit;

namespace DeOlho.ETL.UnitTests
{
    public class TransformsTest
    {
        
        [Fact]
        public async void DescompressStream_Execute()
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    var demoFile = archive.CreateEntry("foo.txt");

                    using (var entryStream = demoFile.Open())
                    using (var streamWriter = new StreamWriter(entryStream))
                    {
                        streamWriter.Write("Bar!");
                    }
                }

                var stepMock = new Mock<Step<Stream>>();
                stepMock
                    .Setup(_ => _.Execute())
                    .ReturnsAsync(new StepValue<Stream>(memoryStream, null));


                var result = await DescompressStreamTransformExtensions.TransformDescompressStream(stepMock.Object).Execute();

                var streamDecompresseds = result.Value.ToList();
                streamDecompresseds.Should().HaveCount(1);
                var streamDecompressed = streamDecompresseds[0];
                streamDecompressed.Name.Should().Be("foo.txt");
                var reader = new StreamReader(streamDecompressed.Stream);
                var text = reader.ReadToEnd();
                text.Should().Be("Bar!");
            }
        }

        [Fact]
        public async void DescompressStream_Collection_Execute()
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    var demoFile = archive.CreateEntry("foo.txt");

                    using (var entryStream = demoFile.Open())
                    using (var streamWriter = new StreamWriter(entryStream))
                    {
                        streamWriter.Write("Bar!");
                    }
                }

                var listStepValue = new List<StepValue<Stream>>() { new StepValue<Stream>(memoryStream, null) };

                var result = await DescompressStreamTransformExtensions.TransformDescompressStream(listStepValue).Execute();

                result.Should().HaveCount(1);
                var streamDecompresseds = result.ToList()[0].Value.ToList();
                streamDecompresseds.Should().HaveCount(1);
                var streamDecompressed = streamDecompresseds[0];
                streamDecompressed.Name.Should().Be("foo.txt");
                var reader = new StreamReader(streamDecompressed.Stream);
                var text = reader.ReadToEnd();
                text.Should().Be("Bar!");
            }
        }

        [Fact]
        public async void CsvToDynamic_Execute()
        {
            var csv = "id,name\r\n1,teste1\r\n2,teste2";
            using(var ms = new MemoryStream())
            {
                using(var sw = new StreamWriter(ms))
                {
                    sw.Write(csv);
                    sw.Flush();
                    ms.Position = 0;
                    
                    var stepMock = new Mock<Step<Stream>>();
                    stepMock
                        .Setup(_ => _.Execute())
                        .ReturnsAsync(new StepValue<Stream>(ms, null));

                    var result = (await CsvToDynamicTransformExtensions.TransformCsvToDynamic(stepMock.Object).Execute());

                    var list = result.Value.ToList();
                    list.Should().HaveCount(2);
                    var item1 = list[0];
                    var item2 = list[1];
                    ((string)item1.id).Should().Be("1");
                    ((string)item1.name).Should().Be("teste1");
                    ((string)item2.id).Should().Be("2");
                    ((string)item2.name).Should().Be("teste2");
                }
            }
        }


        [Fact]
        public async void CsvToDynamic_Collection_Execute()
        {
            var csv = "id,name\r\n1,teste1\r\n2,teste2";
            using(var ms = new MemoryStream())
            {
                using(var sw = new StreamWriter(ms))
                {
                    sw.Write(csv);
                    sw.Flush();
                    ms.Position = 0;
                    var listStepValue = new List<StepValue<Stream>>() { new StepValue<Stream>(ms, null) };

                    var result = (await CsvToDynamicTransformExtensions.TransformCsvToDynamic(listStepValue).Execute()).ToList();

                    result.Should().HaveCount(1);
                    var list = result[0].Value.ToList();
                    list.Should().HaveCount(2);
                    var item1 = list[0];
                    var item2 = list[1];
                    ((string)item1.id).Should().Be("1");
                    ((string)item1.name).Should().Be("teste1");
                    ((string)item2.id).Should().Be("2");
                    ((string)item2.name).Should().Be("teste2");
                }
            }
        }

        [Fact]
        public async void JsonToDynamic_Execute()
        {
            var json = "{ 'id': 1, 'name': 'nome1', 'date': '2019-01-02T15:34:02', 'number': 1.234 }";
            var stepMock = new Mock<IStep<string>>();
            stepMock
                .Setup(_ => _.Execute())
                .ReturnsAsync(new StepValue<string>(json, null));
            var result  = (await JsonToDynamicTransformExtensions.TransformJsonToDynamic(stepMock.Object).Execute()).Value;
            ((int)result.id).Should().Be(1);
            ((string)result.name).Should().Be("nome1");
            ((DateTime)result.date).Year.Should().Be(2019);
            ((DateTime)result.date).Month.Should().Be(1);
            ((DateTime)result.date).Day.Should().Be(2);
            ((DateTime)result.date).Hour.Should().Be(15);
            ((DateTime)result.date).Minute.Should().Be(34);
            ((DateTime)result.date).Day.Should().Be(2);
            ((double)result.number).Should().Be(1.234);
        }

    }
}