using System;
using LogProcessorService;
using UIServiceProtocol;
using BusinessServiceProtocol;
using Attribute4ECAT;
using eCATBusinessServiceProtocol;
using System.IO;
using VTMModelLibrary;
using VTMBusinessActivityBase;
using RemoteTellerServiceProtocol;
using System.Drawing;

namespace IBankProjectBusinessActivity
{
    [GrgActivity("{4E456657-BF10-4B48-8003-904DB1471339}",
                  NodeNameOfConfiguration = "ConfirmSignInfo4Html",
                  Name = "ConfirmSignInfo4Html",
                  Author = "DoDo")]
    public class ConfirmSignInfo4Html : BusinessActivityVTMBase
    {
        #region field
        private const string m_SignalConfirm = "Confirm";
        private const string m_SignalCancel = "Cancel";
        private const string m_SignalLastPage = "LastPage";


        private string customSignPath = string.Empty; //客户签约名字


        #endregion

        #region creating
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new ConfirmSignInfo4Html() as IBusinessActivity;
        }
        #endregion

        #region constructor
        protected ConfirmSignInfo4Html()
        {

        }
        #endregion

        #region method
        protected override emBusActivityResult_t InnerRun(BusinessContext objContext)
        {

            Log.Action.LogDebugFormat("Enter Action: {0}", GetType());
            emBusActivityResult_t emRet = base.InnerRun(objContext);
            if (emRet != emBusActivityResult_t.Success)
            {
                Log.Action.LogError("execute base InnerRun failed");
                VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                VTMContext.ActionResult = emBusActivityResult_t.Failure;
                m_objContext.NextCondition = EventDictionary.s_EventFail;
                return emRet;
            }

            m_objContext.TransactionDataCache.Set("proj_signature", "", GetType());
            SwitchUIState(VTMContext.MainUI, DataDictionary.s_DefaultUIState);
            //添加流水日志
            m_objContext.LogJournal("signature confirm.");

            emWaitSignalResult_t emWaitResult = WaitPopu == 1 ? VTMWaitSignal() : WaitSignal();
            if (emWaitResult == emWaitSignalResult_t.Timeout)
            {
                VTMContext.CurrentTransactionResult = TransactionResult.Cancel;
                VTMContext.ActionResult = emBusActivityResult_t.Timeout;
                VTMContext.NextCondition = EventDictionary.s_EventTimeout;
            }

            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }

        private bool UploadFile(string filePath)
        {
            Log.Action.LogDebug("Enter UploadFile");

            string verifyId = Guid.NewGuid().ToString();
            Log.Action.LogDebug("VerifyID: " + verifyId);
            VTMContext.TransactionDataCache.Set(DataCacheKey.VTM_VTA_CommonVerifyID, verifyId, GetType());

            if (!string.IsNullOrEmpty(filePath))
            {
                try
                {
                    string fileList = filePath.TrimEnd('|');
                    string objbach;
                    var result = VTMContext.MediaPlatformService.UploadFile(fileList, out objbach);
                    if (result)
                    {
                        Log.Action.LogDebug("Succeed to upload file!");
                    }
                    else
                    {
                        Log.Action.LogError("Failed to upload file!");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Log.Action.LogError("Upload photo fail. " + ex.ToString());
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region event

        object _signatureImg = null;
        protected override emBusiCallbackResult_t InnerOnUIEvtHandle(IUIService iUI, UIEventArg objArg)
        {
            emBusiCallbackResult_t result = base.InnerOnUIEvtHandle(iUI, objArg);
            if (emBusiCallbackResult_t.Bypass != result)
            {
                return result;
            }

            if (objArg.EventName.Equals(UIEventNames.s_ClickEvent, StringComparison.OrdinalIgnoreCase))
            {
                if (!(objArg.Key is string)) return emBusiCallbackResult_t.Bypass;
                var key = (string)objArg.Key;
                if (key.Equals(EventDictionary.s_EventConfirm, StringComparison.OrdinalIgnoreCase))
                {

                    Log.Action.LogDebug("Button Click Confirm.");
                    //Thread.Sleep(2000);
                    m_objContext.MainUI.ExecuteCustomCommand(UIServiceCommands.s_updateData, "proj_signature");
                    //Thread.Sleep(100);
                    _signatureImg = m_objContext.GetBindData("proj_signature");
                    if (null != _signatureImg)
                    {
                        //Log.Action.LogDebug("_signatureImg: " + _signatureImg);
                        Base64StringToImage(_signatureImg.ToString());
                        //客户签名的路径
                        var signPath = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp", m_objContext.TerminalConfig.Terminal.ATMNumber + "C.png"));
                        //将用户签名图片写到数据池中
                        VTMContext.TransactionDataCache.Set(DataCacheKey.VTM_CustomSignPath, signPath, GetType());
                        m_objContext.NextCondition = EventDictionary.s_EventContinue;
                        if (VTMContext.RemoteTellerService.EnableRemoteValetMode && !UploadFile(signPath))
                        {
                            m_objContext.NextCondition = EventDictionary.s_EventFail;
                        }

                        SignalCancel();
                        return emBusiCallbackResult_t.Swallowd;
                    }

                    VTMContext.ShowAlarm("Please sign your name beform confirm!", 5);
                    Log.Action.LogDebug("proj_signature is empty");
                    return emBusiCallbackResult_t.Swallowd;
                }
                else if (key.Equals("OnReInput", StringComparison.OrdinalIgnoreCase))
                {
                    Log.Action.LogDebug("Next Step is resign the name.");
                    m_objContext.NextCondition = "OnReInput";
                    return emBusiCallbackResult_t.Swallowd;

                }
                else
                {
                    m_objContext.NextCondition = key;
                    SignalCancel();
                    return emBusiCallbackResult_t.Swallowd;
                }
            }


            return emBusiCallbackResult_t.Bypass;
        }
        #endregion


        #region transfer the string to img
        /// <summary>
        /// 将字符串转化为jpg图片并保存
        /// </summary>
        /// <param name="stringImgPath">base64字符串</param>
        private void Base64StringToImage(string stringImgPath)
        {
            try
            {
                stringImgPath = stringImgPath.Split(',')[1];
                byte[] arr = Convert.FromBase64String(stringImgPath);
                var ms = new MemoryStream(arr);
                var bmp = new Bitmap(ms);
                bmp.Save(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp", m_objContext.TerminalConfig.Terminal.ATMNumber+"C.png"), System.Drawing.Imaging.ImageFormat.Png);
                //var signPath = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp", m_objContext.TerminalConfig.Terminal.ATMNumber + "C.png"));
                ms.Dispose();
                bmp.Dispose();
                Log.Action.LogDebug("Transfer successe.");
            }
            catch (ArgumentException ex)
            {
                Log.Action.LogError("Fail to transfer img, the exception is:" + ex);
            }
            catch (ArithmeticException ex)
            {
                Log.Action.LogError("Fail to transfer img, the exception is:" + ex);
            }
            catch (StackOverflowException ex)
            {
                Log.Action.LogError("Fail to transfer img, the exception is:" + ex);
            }
            catch (System.IO.IOException ex)
            {
                Log.Action.LogError("Fail to transfer img, the exception is:" + ex);
            }
            catch (IndexOutOfRangeException ex)
            {
                Log.Action.LogError("Fail to transfer img, the exception is:" + ex);
            } 
            catch (FormatException ex)
            {
                Log.Action.LogError("Fail to transfer img, the exception is:" + ex);
            }
            catch (System.Runtime.InteropServices.ExternalException ex)
            {
                Log.Action.LogError("Fail to transfer img, the exception is:" + ex);
            }

        }

        #endregion

    }
}