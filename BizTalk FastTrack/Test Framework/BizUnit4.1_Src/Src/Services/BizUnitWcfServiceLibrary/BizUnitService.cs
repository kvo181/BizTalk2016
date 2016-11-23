using BizUnit;
using BizUnit.Xaml;
using System;
using System.ServiceModel;

namespace BizUnitWcfServiceLibrary
{
    public class BizUnitService : IBizUnitService
    {
        //[OperationBehavior(Impersonation = ImpersonationOption.Required)]
        public void HostConductorStep(HostConductorStep step)
        {
            var identity = GetIdentity();
            BizUnit.Context ctx = new Context(new RemoteTestLogger());

            var testCase = new TestCase();
            var hostConductorStep = new BizUnit.TestSteps.BizTalk.Host.HostConductorStep
            {
                Action = step.Action,
                GrantLogOnAsService = step.GrantLogOnAsService,
                HostInstanceName = step.HostInstanceName,
                Logon = step.Logon,
                PassWord = step.PassWord,
                Servers = step.Servers
            };
            testCase.ExecutionSteps.Add(hostConductorStep);
            var bizUnit = new BizUnit.BizUnit(testCase, ctx);
            try
            {
                bizUnit.RunTest();
            }
            catch (TestStepExecutionException tex)
            {
                var ex = tex.InnerException ?? tex;
                throw new Exception(string.Format("{0}-{1}", identity, ex.Message), ex);
            }
        }

        public void OrchestrationConductorStep(OrchestrationConductorStep step)
        {
            var identity = GetIdentity();
            BizUnit.Context ctx = new Context(new RemoteTestLogger());

            BizUnit.TestSteps.BizTalk.Orchestration.OrchestrationConductorStep.OrchestrationAction action;
            switch (step.Action)
            {
                case OrchestrationAction.Start:
                    action = BizUnit.TestSteps.BizTalk.Orchestration.OrchestrationConductorStep.OrchestrationAction.Start;
                    break;
                default:
                    action = BizUnit.TestSteps.BizTalk.Orchestration.OrchestrationConductorStep.OrchestrationAction.Stop;
                    break;
            }

            var testCase = new TestCase();
            var orchestrationConductorStep = new BizUnit.TestSteps.BizTalk.Orchestration.OrchestrationConductorStep
                                                 {
                                                     Action = action,
                                                     AssemblyName = step.AssemblyName,
                                                     DelayForCompletion = step.DelayForCompletion,
                                                     OrchestrationName = step.OrchestrationName
                                                 };
            testCase.ExecutionSteps.Add(orchestrationConductorStep);
            var bizUnit = new BizUnit.BizUnit(testCase, ctx);
            try
            {
                bizUnit.RunTest();
            }
            catch (TestStepExecutionException tex)
            {
                var ex = tex.InnerException ?? tex;
                throw new Exception(string.Format("{0}-{1}", identity, ex.Message), ex);
            }
        }

        public void ReceiveLocationEnabledStep(ReceiveLocationEnabledStep step)
        {
            var identity = GetIdentity();
            BizUnit.Context ctx = new Context(new RemoteTestLogger());

            var testCase = new TestCase();
            var receiveLocationEnabledStep = new BizUnit.TestSteps.BizTalk.Port.ReceiveLocationEnabledStep
                                                 {
                                                     IsDisabled = step.IsDisabled,
                                                     ReceiveLocationName = step.ReceiveLocationName
                                                 };
            testCase.ExecutionSteps.Add(receiveLocationEnabledStep);
            var bizUnit = new BizUnit.BizUnit(testCase, ctx);
            try
            {
                bizUnit.RunTest();
            }
            catch (TestStepExecutionException tex)
            {
                var ex = tex.InnerException ?? tex;
                throw new Exception(string.Format("{0}-{1}", identity, ex.Message), ex);
            }
        }

        public void ReceivePortConductorStep(ReceivePortConductorStep step)
        {
            var identity = GetIdentity();
            var ctx = new Context(new RemoteTestLogger());

            BizUnit.TestSteps.BizTalk.Port.ReceivePortConductorStep.ReceivePortAction action;
            switch (step.Action)
            {
                case ReceivePortAction.Enable:
                    action = BizUnit.TestSteps.BizTalk.Port.ReceivePortConductorStep.ReceivePortAction.Enable;
                    break;
                default:
                    action = BizUnit.TestSteps.BizTalk.Port.ReceivePortConductorStep.ReceivePortAction.Disable;
                    break;
            }

            var testCase = new TestCase();
            var receivePortConductorStep = new BizUnit.TestSteps.BizTalk.Port.ReceivePortConductorStep
                                               {
                                                   ReceivePortName = step.ReceivePortName,
                                                   ReceiveLocationName = step.ReceiveLocationName,
                                                   DelayForCompletion = step.DelayForCompletion,
                                                   Action = action
                                               };

            testCase.ExecutionSteps.Add(receivePortConductorStep);
            var bizUnit = new BizUnit.BizUnit(testCase, ctx);
            try
            {
                bizUnit.RunTest();
            }
            catch (TestStepExecutionException tex)
            {
                var ex = tex.InnerException ?? tex;
                throw new Exception(string.Format("{0}-{1}", identity, ex.Message), ex);
            }
        }

        public void SendPortConductorStep(SendPortConductorStep step)
        {
            var identity = GetIdentity();
            var ctx = new Context(new RemoteTestLogger());

            BizUnit.TestSteps.BizTalk.Port.SendPortConductorStep.SendPortAction action;
            switch (step.Action)
            {
                case SendPortAction.Start:
                    action = BizUnit.TestSteps.BizTalk.Port.SendPortConductorStep.SendPortAction.Start;
                    break;
                case SendPortAction.Stop:
                    action = BizUnit.TestSteps.BizTalk.Port.SendPortConductorStep.SendPortAction.Stop;
                    break;
                default:
                    action = BizUnit.TestSteps.BizTalk.Port.SendPortConductorStep.SendPortAction.Unenlist;
                    break;
            }

            var testCase = new TestCase();
            var sendPortConductorStep = new BizUnit.TestSteps.BizTalk.Port.SendPortConductorStep
            {
                SendPortName = step.SendPortName,
                DelayForCompletion = step.DelayForCompletion,
                Action = action
            };

            testCase.ExecutionSteps.Add(sendPortConductorStep);
            var bizUnit = new BizUnit.BizUnit(testCase, ctx);
            try
            {
                bizUnit.RunTest();
            }
            catch (TestStepExecutionException tex)
            {
                var ex = tex.InnerException ?? tex;
                throw new Exception(string.Format("{0}-{1}", identity, ex.Message), ex);
            }
        }

        public void SendPortGroupConductorStep(SendPortGroupConductorStep step)
        {
            var identity = GetIdentity();
            var ctx = new Context(new RemoteTestLogger());

            BizUnit.TestSteps.BizTalk.Port.SendPortGroupConductorStep.SendPortGroupAction action;
            switch (step.Action)
            {
                case SendPortGroupAction.Start:
                    action = BizUnit.TestSteps.BizTalk.Port.SendPortGroupConductorStep.SendPortGroupAction.Start;
                    break;
                default:
                    action = BizUnit.TestSteps.BizTalk.Port.SendPortGroupConductorStep.SendPortGroupAction.Stop;
                    break;
            }

            var testCase = new TestCase();
            var sendPortGroupConductorStep = new BizUnit.TestSteps.BizTalk.Port.SendPortGroupConductorStep
            {
                SendPortGroupName = step.SendPortGroupName,
                DelayForCompletion = step.DelayForCompletion,
                Action = action
            };

            testCase.ExecutionSteps.Add(sendPortGroupConductorStep);
            var bizUnit = new BizUnit.BizUnit(testCase, ctx);
            try
            {
                bizUnit.RunTest();
            }
            catch (TestStepExecutionException tex)
            {
                var ex = tex.InnerException ?? tex;
                throw new Exception(string.Format("{0}-{1}", identity, ex.Message), ex);
            }
        }

        public string GetData(int value)
        {
            var identity = GetIdentity();

            var result = string.Format("You entered: {0}", value);

            return identity + "--" + result;
        }

        static string GetIdentity()
        {
            var windowsIdentity =
                ServiceSecurityContext.Current.WindowsIdentity;
            if (windowsIdentity == null)
                throw new InvalidOperationException("The caller cannot be mapped to a windows identity");

            var primaryIdentity =
                ServiceSecurityContext.Current.PrimaryIdentity;

            var identity =
                string.Format("IsAnonymous:{0}, Name:{1}, ImpersonationLevel:{2}, Identity:{3}, AuthenticationType:{4}",
                              ServiceSecurityContext.Current.IsAnonymous,
                              windowsIdentity.Name,
                              windowsIdentity.ImpersonationLevel,
                              primaryIdentity.Name,
                              primaryIdentity.AuthenticationType);
            return identity;
        }

    }
}
