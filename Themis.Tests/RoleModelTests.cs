//using System;
//using System.Linq;
//using NUnit.Framework;
//using Rhino.Mocks;
//using Rhino.Mocks.Constraints;
//using Themis.Model;
//using Themis.Tests.TestData;

//namespace Themis.Tests
//{
//    [TestFixture]
//    public class RoleModelTests
//    {
//        [Test]
//        public void Evaluate_WhenDifferenTDemandsConfigured_ThenOnlySpecificAreCalled( )
//        {
//            var r = new Role();
//            var pa = new ADemand();

//            var mocks = new MockRepository();

//            var evaluator = mocks.Stub<IEvaluator>();
//            var evaluatorResult = new object();
//            Expect.Call(evaluator.DemandType)
//                .Return(typeof(ADemand));
//            Expect.Call(evaluator.Evaluate(null, null))
//                .Constraints(new Same(pa), new Same(r))
//                .Return(evaluatorResult);

//            var evaluator2 = mocks.StrictMock<IEvaluator>();
//            Expect.Call(evaluator2.DemandType)
//                .Return(typeof(BDemand));

//            mocks.ReplayAll();

//            var test = new RoleModel<Role>(new[] { evaluator, evaluator2 });
//            var result = test.Evaluate(pa, r).ToArray();

//            mocks.VerifyAll();

//            Assert.AreEqual(1, result.Count(),
//                            "The number of returned results should be equal to number of evaluators for the permission");
//            Assert.Contains(evaluatorResult, result,
//                            "One evaluator for APermission, hence the result should contain this result");
//        }

//        [Test]
//        public void Evaluate_WhenNoPermissionsProvided_ThenReturnsEmptyResults( )
//        {
//            var r = new Role();
//            var model = new RoleModel<Role>(Enumerable.Empty<IEvaluator>());
//            var result = model.Evaluate(new ADemand(), r);

//            Assert.AreEqual(0, result.Count(), "Model returns result from no evaluators");
//        }

//        [Test]
//        [ExpectedException(typeof(ArgumentException))]
//        public void Evaluate_WhenOtherRoleTypePassed_Throws( )
//        {
//            var model = new RoleModel<Role>(Enumerable.Empty<IEvaluator>());
//            model.Evaluate(new ADemand(), string.Empty);
//        }

//        [Test]
//        public void Evaluate_WhenPermissionEvaluated_ThenEachEvaluatorReturnsValue( )
//        {
//            var r = new Role();
//            var p = new ADemand();

//            var mocks = new MockRepository();

//            var evaluator = mocks.Stub<IEvaluator>();
//            var evaluatorResult = new object();
//            Expect.Call(evaluator.DemandType)
//                .Return(typeof(ADemand));
//            Expect.Call(evaluator.Evaluate(null, null))
//                .Constraints(new Same(p), new Same(r))
//                .Return(evaluatorResult);

//            var evaluator2 = mocks.Stub<IEvaluator>();
//            var evaluatorResult2 = new object();
//            Expect.Call(evaluator2.DemandType)
//                .Return(typeof(ADemand));
//            Expect.Call(evaluator2.Evaluate(null, null))
//                .Constraints(new Same(p), new Same(r))
//                .Return(evaluatorResult2);

//            mocks.ReplayAll();

//            var test = new RoleModel<Role>(new[] { evaluator, evaluator2 });
//            var result = test.Evaluate(p, r).ToArray();

//            mocks.VerifyAll();

//            Assert.AreEqual(2, result.Count(), "The number of returned results should be equal to number of evaluators");
//            Assert.Contains(evaluatorResult, result, "One result not included");
//            Assert.Contains(evaluatorResult2, result, "One result not included");
//        }

//        [Test]
//        public void GetEvaluatorsCount_ReturnsTheRightNumberOfEvaluators( )
//        {
//            var mocks = new MockRepository();
//            var pa = new ADemand();
//            var pb = new BDemand();

//            var evaluatorA1 = mocks.Stub<IEvaluator>();
//            Expect.Call(evaluatorA1.DemandType)
//                .Return(typeof(ADemand));

//            var evaluatorA2 = mocks.Stub<IEvaluator>();
//            Expect.Call(evaluatorA2.DemandType)
//                .Return(typeof(ADemand));

//            var evaluatorA3 = mocks.Stub<IEvaluator>();
//            Expect.Call(evaluatorA3.DemandType)
//                .Return(typeof(ADemand));

//            var evaluatorB1 = mocks.Stub<IEvaluator>();
//            Expect.Call(evaluatorB1.DemandType)
//                .Return(typeof(BDemand));

//            var evaluatorB2 = mocks.Stub<IEvaluator>();
//            Expect.Call(evaluatorB2.DemandType)
//                .Return(typeof(BDemand));

//            mocks.ReplayAll();

//            var test = new RoleModel<Role>(new[] { evaluatorA1, evaluatorA2, evaluatorA3 });
//            Assert.AreEqual(3, test.GetEvaluatorsCount(typeof(ADemand)));
//            Assert.AreEqual(0, test.GetEvaluatorsCount(typeof(BDemand)));

//            var test2 = new RoleModel<OtherRole>(new[] { evaluatorA1, evaluatorA2, evaluatorB1, evaluatorB2 });
//            Assert.AreEqual(2, test2.GetEvaluatorsCount(typeof(ADemand)));
//            Assert.AreEqual(2, test2.GetEvaluatorsCount(typeof(BDemand)));

//            var test3 = new RoleModel<Role>(new[] { evaluatorB1, evaluatorB2 });
//            Assert.AreEqual(0, test3.GetEvaluatorsCount(typeof(ADemand)));
//            Assert.AreEqual(2, test3.GetEvaluatorsCount(typeof(BDemand)));

//            mocks.VerifyAll();
//        }
//    }
//}