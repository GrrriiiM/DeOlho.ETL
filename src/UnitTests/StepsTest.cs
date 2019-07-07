using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;

namespace DeOlho.ETL.UnitTests
{
    public class StepsTest
    {
        public class Teste1
        {
            public int Id { get; set; }
            public string Value { get; set; }
            public DateTime Date { get; set; }
        }

        public class Teste2
        {
            public string Concat { get; set; }
        }
        
        [Fact]
        public async void Step_Transform()
        {
            var text = "texto teste";
            
            var stepMock = new Mock<Step<string>>();
            stepMock.Setup(_ => _.Execute())
                .ReturnsAsync(new StepValue<string>(text, null));

            var result = await stepMock.Object.Transform(_ => _.Value.ToUpper()).Execute();

            result.Value.Should().Be("TEXTO TESTE");
            result.Parent.Value.Should().Be("texto teste");
        }

        [Fact]
        public async void Step_Collection_Transform()
        {
            var text1 = "texto1";
            var text2 = "texto2";
            
            var list = new List<StepValue<string>> { new StepValue<string>(text1, null), new StepValue<string>(text2, null) };

            var result = await list.Transform(_ => _.Value.ToUpper()).Execute();

            result.Should().HaveCount(2);
            var item1 = result.ToList()[0];
            item1.Value.Should().Be("TEXTO1");
            item1.Parent.Value.Should().Be("texto1");
            var item2 = result.ToList()[1];
            item2.Value.Should().Be("TEXTO2");
            item2.Parent.Value.Should().Be("texto2");
        }

        [Fact]
        public async void Step_TransformAsync()
        {
            var text = "texto teste";
            
            var stepMock = new Mock<Step<string>>();
            stepMock.Setup(_ => _.Execute())
                .ReturnsAsync(new StepValue<string>(text, null));

            var result = await stepMock.Object.TransformAsync(async _ => { await Task.CompletedTask; return _.Value.ToUpper(); }).Execute();

            result.Value.Should().Be("TEXTO TESTE");
            result.Parent.Value.Should().Be("texto teste");
        }

        [Fact]
        public async void Step_TransformToList()
        {
            var test = new {
                list = new [] {
                    new {
                        text = "texto1"
                    },
                    new {
                        text = "texto2"
                    },
                }
            };
            
            var stepMock = new Mock<Step<dynamic>>();
            stepMock.Setup(_ => _.Execute())
                .ReturnsAsync(new StepValue<dynamic>(test, null));

            var result = await stepMock.Object.TransformToList(_ => new List<dynamic>(_.Value.list)).Execute();

            result.Should().HaveCount(2);
            ((string)result.ToList()[0].Value.text).Should().Be("texto1");
            ((string)result.ToList()[1].Value.text).Should().Be("texto2");
            
        }


        [Fact]
        public async void Step_Collection_TransformToList()
        {
            var test1 = new {
                list = new [] {
                    new {
                        text = "texto1"
                    },
                    new {
                        text = "texto2"
                    },
                }
            };

            var test2 = new {
                list = new [] {
                    new {
                        text = "texto3"
                    },
                    new {
                        text = "texto4"
                    },
                }
            };
            
            
            var list = new List<StepValue<dynamic>>() { new StepValue<dynamic>(test1, null), new StepValue<dynamic>(test2, null) };
            var result = await list.TransformToList(_ => new List<dynamic>(_.Value.list)).Execute();

            result.Should().HaveCount(4);
            ((string)result.ToList()[0].Value.text).Should().Be("texto1");
            ((string)result.ToList()[1].Value.text).Should().Be("texto2");
            ((string)result.ToList()[2].Value.text).Should().Be("texto3");
            ((string)result.ToList()[3].Value.text).Should().Be("texto4");
            
        }


        [Fact]
        public async void Step_Extract()
        {
            var test = "testando";
            
            var sourceMock = new Mock<ISource<string>>();
            sourceMock.Setup(_ => _.Execute())
                .ReturnsAsync(test);

            var stepMock = new Mock<Step<string>>();
            stepMock.Setup(_ => _.Execute())
                .ReturnsAsync(new StepValue<string>("oi", null));

            var result = await stepMock.Object.Extract(_ => sourceMock.Object).Execute();

            result.Value.Should().Be("testando");
            result.Parent.Value.Should().Be("oi");
        }



        [Fact]
        public async void StepTransform_Execute()
        {
            var teste1 = new Teste1{
                Id = 1,
                Value = "Value",
                Date = new DateTime(2000,1,1)
            };

            var stepMock = new Mock<IStep<Teste1>>();
            stepMock.Setup(_ => _.Execute())
                .ReturnsAsync(() => new StepValue<Teste1>(
                    teste1,
                    null));

            var stepTransform = new StepTransform<Teste1, Teste2>(stepMock.Object, 
                _ => Task.Run(() => new Teste2 { Concat = $"{_.Value.Id}{_.Value.Value}{_.Value.Date}" }));

            var result = await stepTransform.Execute();

            result.TypeValue.Should().Be(typeof(Teste2));
            result.Value.Concat.Should().Be($"{teste1.Id}{teste1.Value}{teste1.Date}");
            result.Parent.TypeValue.Should().Be(typeof(Teste1));
            ((Teste1)result.Parent.Value).Should().BeEquivalentTo(teste1);
        }

    }
}