using Attribute4ECAT;
using BusinessServiceProtocol;
using LogProcessorService;
using ResourceManagerProtocol;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTMBusinessActivityBase;
using VTMModelLibrary;

namespace VTMBusinessActivity
{
    [GrgActivity("{ED61F645-4501-4291-8E52-9FD0447FB090}",
            NodeNameOfConfiguration = "SetIdTestData",
            Name = "SetIdTestData",
            Author = "hzlin9")]
    public class SetIdTestData : BusinessActivityVTMBase
    {
        #region creating
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new SetIdTestData() as IBusinessActivity;
        }
        #endregion

        #region constructor
        protected SetIdTestData()
        {

        }
        #endregion

        #region property
        private ScanIDCardInfo currentIDCard = new ScanIDCardInfo(); //身份证信息
        public ScanIDCardInfo VTM_IDCARD
        {
            get { return currentIDCard; }
            set { currentIDCard = value; }
        }
        #endregion


        #region method
        protected override emBusActivityResult_t InnerRun(BusinessContext objContext)
        {
            Log.Action.LogDebugFormat("Enter Action: {0}", GetType());
            emBusActivityResult_t result = base.InnerRun(objContext);

            if (emBusActivityResult_t.Success != result)
            {
                Log.Action.LogError("Failed to run base's implement");
                return result;
            }

            VTM_IDCARD.IdCard_PhotoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "idtest", "Id_photo.jpg");
            VTM_IDCARD.IdCard_Name = "小明";

            VTM_IDCARD.IdCard_Sex = "男";
            VTM_IDCARD.IdCard_Nation = "汉";

            VTM_IDCARD.IdCard_Birthday = "20000101";
            VTM_IDCARD.IdCard_Year = VTM_IDCARD.IdCard_Birthday.Substring(0, 4);
            VTM_IDCARD.IdCard_Month = VTM_IDCARD.IdCard_Birthday.Substring(4, 2);
            VTM_IDCARD.IdCard_Day = VTM_IDCARD.IdCard_Birthday.Substring(6, 2);

            VTM_IDCARD.IdCard_Address = "广州市";
            VTM_IDCARD.IdCard_IDNo = "442343200001011234";
            VTM_IDCARD.IdCard_IDOrg = "广州市公安局";
            VTM_IDCARD.IdCard_BeginDate = "20100101";
            VTM_IDCARD.IdCard_EndDate = "20200101";

            VTM_IDCARD.IdCard_ScanImg1 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp",
                m_objContext.TerminalConfig.Terminal.ATMNumber + "A.jpg");

            VTM_IDCARD.IdCard_ScanImg2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp",
                m_objContext.TerminalConfig.Terminal.ATMNumber + "B.jpg");

            GenerateIdCardFrontImage();

            GenerateIdCardBackImage();

            m_objContext.TransactionDataCache.Set(DataCacheKey.s_coreIDCardFgImgPath, VTM_IDCARD.IdCard_ScanImg1, GetType());

            m_objContext.TransactionDataCache.Set(DataCacheKey.s_coreIDCardBgImgPath, VTM_IDCARD.IdCard_ScanImg2, GetType());

            m_objContext.TransactionDataCache.Set(DataCacheKey.VTM_IDCARD, VTM_IDCARD, GetType());

            m_objContext.NextCondition = "OnConfirm";
            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }

        #endregion

        private bool GenerateIdCardFrontImage()
        {
            try
            {
                Log.Action.LogDebug("Generate ID card front image");
                using (Image img = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + @"Resource\Common\Image\CN\Common\idcard_front_bg.png"))
                {
                    using (Image img2 = Image.FromFile(VTM_IDCARD.IdCard_PhotoPath))
                    {
                        using (Graphics g = Graphics.FromImage(img))
                        {
                            using (Font fnt = new Font("微软雅黑", 22, GraphicsUnit.Pixel))
                            {
                                g.SmoothingMode = SmoothingMode.HighQuality;
                                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                                g.DrawString(VTM_IDCARD.IdCard_Name, fnt, Brushes.Black, new PointF(98, 56));
                                var fnt2 = new Font("微软雅黑", 20, GraphicsUnit.Pixel);
                                g.DrawString(VTM_IDCARD.IdCard_Sex, fnt2, Brushes.Black, new PointF(98, 98));
                                g.DrawString(VTM_IDCARD.IdCard_Nation, fnt2, Brushes.Black, new PointF(228, 98));
                                g.DrawString(VTM_IDCARD.IdCard_Year, fnt2, Brushes.Black, new PointF(98, 139));
                                g.DrawString(VTM_IDCARD.IdCard_Month, fnt2, Brushes.Black, new PointF(191, 139));
                                g.DrawString(VTM_IDCARD.IdCard_Day, fnt2, Brushes.Black, new PointF(247, 139));
                                int addressLine = Convert.ToInt32(Math.Ceiling(VTM_IDCARD.IdCard_Address.Length / 10.0));
                                for (int i = 0; i < addressLine; i++)
                                {
                                    if (i != addressLine - 1)
                                    {
                                        g.DrawString(VTM_IDCARD.IdCard_Address.Substring(i * 10, 10), fnt, Brushes.Black, new PointF(98, 178 + i * 25));
                                    }
                                    else
                                    {
                                        g.DrawString(VTM_IDCARD.IdCard_Address.Substring(i * 10), fnt, Brushes.Black, new PointF(98, 178 + i * 25));
                                    }
                                }
                                StringBuilder idBuilder = new StringBuilder();
                                foreach (var ch in VTM_IDCARD.IdCard_IDNo)
                                {
                                    idBuilder.Append(ch);
                                    idBuilder.Append(' ');
                                }
                                idBuilder.Remove(idBuilder.Length - 1, 1);
                                var fnt3 = new Font("Arial", 22, GraphicsUnit.Pixel);
                                g.DrawString(idBuilder.ToString(), fnt3, Brushes.Black, new PointF(190, 288));

                                ImageAttributes attri = new ImageAttributes();
                                Color transparentClr = Color.FromArgb(254, 254, 254);
                                attri.SetColorKey(transparentClr, transparentClr);
                                g.DrawImage(img2, new Rectangle(360, 56, 156, 208), 0, 0, img2.Width, img2.Height, GraphicsUnit.Pixel, attri);

                                g.Flush();
                            }
                        }

                        img.Save(VTM_IDCARD.IdCard_ScanImg1, ImageFormat.Png);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Action.LogError(ex.Message, ex);
                return false;
            }

            return true;
        }

        private bool GenerateIdCardBackImage()
        {
            try
            {
                Log.Action.LogDebug("Generate ID card back image");
                using (Image img = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + @"Resource\Common\Image\CN\Common\idcard_back_bg.png"))
                {
                    using (Graphics g = Graphics.FromImage(img))
                    {
                        g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                        using (Font fnt = new Font("微软雅黑", 20, GraphicsUnit.Pixel))
                        {
                            var fnt1 = new Font("Arial", 20, GraphicsUnit.Pixel);
                            g.DrawString(VTM_IDCARD.IdCard_IDOrg, fnt, Brushes.Black, new PointF(220, 245));

                            string dateformat = m_objContext.CurrentUIResource.LoadString("IDS_DateFormater", TextCategory.s_UI);
                            string enddtime = string.Empty;
                            if (currentIDCard.IdCard_EndDate.Equals("长期"))
                            {
                                enddtime = "长期";
                            }
                            else
                            {
                                enddtime = DateTime.ParseExact(currentIDCard.IdCard_EndDate, "yyyyMMdd", null).ToString(dateformat);
                            }

                            DateTime begindtime = DateTime.ParseExact(currentIDCard.IdCard_BeginDate, "yyyyMMdd", null);
                            string IdCard_ValidateDate = string.Concat(begindtime.ToString(dateformat), m_objContext.CurrentUIResource.LoadString("IDS_DateConcat", TextCategory.s_UI), enddtime);

                            g.DrawString(IdCard_ValidateDate, fnt1, Brushes.Black, new PointF(220, 290));
                            g.Flush();
                        }
                        img.Save(VTM_IDCARD.IdCard_ScanImg2, ImageFormat.Png);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Action.LogError(ex.Message, ex);
                return false;
            }

            return true;
        }

    }
}
