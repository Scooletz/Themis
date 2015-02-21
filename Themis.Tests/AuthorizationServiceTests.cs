//using System.Collections.Generic;
//using System.Linq;
//using NUnit.Framework;
//using Rhino.Mocks;
//using Themis.Impl;
//using Themis.Model;
//using Themis.Tests.TestData;

//namespace Themis.Tests
//{
//    [TestFixture]
//    public class AuthorizationServiceTests
//    {
//        [Test]
//        public void Evaluate_WhenRolesPassed_ThenOnlyModelsForPassedRolesAreUsed( )
//        {
//            var mocks = new MockRepository();
//            var p = new ADemand();
//            var r = new Role();
//            var otherr = new OtherRole();
//            var roleModel = mocks.Stub<IRoleModel>();

//            var rpermissionResult1 = new object();
//            var rpermissionResult2 = new object();
//            var rpermissionResult3 = new object();
//            Expect.Call(roleModel.RoleType)
//                .Return(typeof(Role));
//            Expect.Call(roleModel.Evaluate(p, r))
//                .Return(new[] { rpermissionResult1, rpermissionResult2, rpermissionResult3 });

//            var otherRoleModel = mocks.Stub<IRoleModel>();

//            var otherrpermissionResult = new object();
//            Expect.Call(otherRoleModel.RoleType)
//                .Return(typeof(OtherRole));
//            Expect.Call(otherRoleModel.Evaluate(p, otherr))
//                .Return(new[] { otherrpermissionResult });

//            mocks.ReplayAll();

//            var test = new DemandService(new[] { roleModel, otherRoleModel });

//            var roleEvaluationResult = test.Evaluate<ADemand, object>(p, r); // only first role
//            var otherRoleEvaluationResult = test.Evaluate<ADemand, object>(p, otherr); // only the second
//            var bothRolesEvaluationResult = test.Evaluate<ADemand, object>(p, r, otherr); // both

//            mocks.VerifyAll();

//            // first
//            Assert.AreEqual(3, roleEvaluationResult.Count());
//            Assert.Contains(rpermissionResult1, roleEvaluationResult.ToArray());
//            Assert.Contains(rpermissionResult2, roleEvaluationResult.ToArray());
//            Assert.Contains(rpermissionResult3, roleEvaluationResult.ToArray());

//            // second
//            Assert.AreEqual(1, otherRoleEvaluationResult.Count());
//            Assert.Contains(otherrpermissionResult, otherRoleEvaluationResult.ToArray());

//            // both
//            Assert.AreEqual(4, bothRolesEvaluationResult.Count());
//            Assert.Contains(rpermissionResult1, bothRolesEvaluationResult.ToArray());
//            Assert.Contains(rpermissionResult2, bothRolesEvaluationResult.ToArray());
//            Assert.Contains(rpermissionResult3, bothRolesEvaluationResult.ToArray());
//            Assert.Contains(otherrpermissionResult, otherRoleEvaluationResult.ToArray());
//        }

//        [Test]
//        [ExpectedException]
//        public void Evaluate_WhenTwoModelsForTheSameRolePassed_ThenThrows( )
//        {
//            var mocks = new MockRepository();

//            var roleModel = mocks.Stub<IRoleModel>();
//            Expect.Call(roleModel.RoleType)
//                .Return(typeof(Role));

//            var roleModel2 = mocks.Stub<IRoleModel>();
//            Expect.Call(roleModel2.RoleType)
//                .Return(typeof(Role));

//            mocks.ReplayAll();

//            new DemandService(new[] { roleModel, roleModel2 });

//            mocks.VerifyAll();
//        }

//        [Test]
//        public void HasAnyEvaluators_WhenCalled_ThenReturnsBoolAccordingToCountOfEvaluatorsForDemannd( )
//        {
//            var mocks = new MockRepository();

//            var m = mocks.Stub<IRoleModel>();
//            Expect.Call(m.RoleType)
//                .Return(typeof(Role));
//            Expect.Call(m.GetEvaluatorsCount(typeof(ADemand)))
//                .Return(1);
//            Expect.Call(m.GetEvaluatorsCount(typeof(BDemand)))
//                .Return(0);

//            mocks.ReplayAll();

//            var service = new DemandService(new[] { m });

//            Assert.True(service.HasAnyEvaluators<ADemand, object>(new Role()));
//            Assert.False(service.HasAnyEvaluators<BDemand, IEnumerable<string>>(new Role()));
//        }
//    }
//}