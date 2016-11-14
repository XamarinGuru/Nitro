
using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.IO;
using Android.Graphics;
using Android.Provider;

namespace goheja
{
    public class FragmentProfile : Android.Support.V4.App.Fragment
    {
        ImageView meImage;
        TextView tvFirstName;
        TextView tvLastName;
        TextView tvEmail;
        TextView tvPhone;
		TextView lblUsername;
        byte[] bitmapByteData = { 0 };

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
			return inflater.Inflate(Resource.Layout.fProfile, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

			meImage = view.FindViewById<ImageView>(Resource.Id.ivTest);
			lblUsername = view.FindViewById<TextView>(Resource.Id.lblUsername);
            tvFirstName = view.FindViewById<TextView>(Resource.Id.etFirstName);
            tvLastName = view.FindViewById<TextView>(Resource.Id.etlastName);
            tvEmail = view.FindViewById<TextView>(Resource.Id.etMail);
            tvPhone = view.FindViewById<TextView>(Resource.Id.etPhone);

			view.FindViewById<Button>(Resource.Id.perImage).Click += setImage_OnClick;
			view.FindViewById<Button>(Resource.Id.btnSave).Click += saveData_OnClick;
            view.FindViewById<Button>(Resource.Id.seriousBtn).Click += seriousBtn__OnClick;
			view.FindViewById<Button>(Resource.Id.resetCalBtn).Click += resetCalBtn__OnClick;

            setBitmapImg();
            setPersonalData();
        }

		private void setImage_OnClick(object sender, EventArgs e)
		{
			var imageIntent = new Intent();
			imageIntent.SetType("image/*");
			imageIntent.SetAction(Intent.ActionGetContent);
			StartActivityForResult(Intent.CreateChooser(imageIntent, "Select photo"), 0);
		}
        private void saveData_OnClick(object sender, EventArgs e)
        {
			try
			{
				var contextPref = Application.Context.GetSharedPreferences("goheja", FileCreationMode.Private);
				trackSvc.Service1 sv = new trackSvc.Service1();
				contextPref = Application.Context.GetSharedPreferences("goheja", FileCreationMode.Private);

				sv.updateAthPersonalData(contextPref.GetString("storedAthId", "0").ToString(), tvFirstName.Text, tvLastName.Text, tvPhone.Text, tvEmail.Text, bitmapByteData);

				Toast.MakeText(Activity, "Data Saved! We hope you got serious...", ToastLength.Long).Show();
			}
			catch
			{
				Toast.MakeText(Activity, "Error saving data!", ToastLength.Long).Show();
			}
        }

        private void seriousBtn__OnClick(object sender, EventArgs e)
        {
            var activity2go = new Intent(Activity, typeof(ProfileWebViewActivity));
            activity2go.PutExtra("MyData", "Data from Activity1");
            StartActivity(activity2go);
        }

		#region remove existing Nitro calendar
		private void resetCalBtn__OnClick(object sender, EventArgs e)
		{
			try
			{
				var calendarsUri = CalendarContract.Calendars.ContentUri;

				string[] calendarsProjection = {
				   CalendarContract.Calendars.InterfaceConsts.Id,
				   CalendarContract.Calendars.InterfaceConsts.CalendarDisplayName,
				   CalendarContract.Calendars.InterfaceConsts.AccountName
				};

				var cursor = this.Activity.ApplicationContext.ContentResolver.Query(calendarsUri, calendarsProjection, null, null, null);

				if (cursor.MoveToFirst())
				{
					do
					{
						long id = cursor.GetLong(0);
						String displayName = cursor.GetString(1);
						if (displayName == "Nitro Calendar")
							RemoveCalendar(id);
					} while (cursor.MoveToNext());
				}
			}
			catch
			{
			}
		}

		private void RemoveCalendar(long calID)
		{
			try{
				Android.Net.Uri.Builder builder1 = CalendarContract.Calendars.ContentUri.BuildUpon();
				builder1.AppendQueryParameter(CalendarContract.Calendars.InterfaceConsts.CalendarDisplayName, "Nitro Calendar");

				String[] selArgs = new String[] { "Nitro Calendar" };
				this.Activity.ContentResolver.Delete(CalendarContract.Calendars.ContentUri, CalendarContract.Calendars.InterfaceConsts.CalendarDisplayName + " =? ", selArgs);
			}
			catch
			{
			}
		}
		#endregion

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
			try
			{
				if (resultCode == (int)Result.Ok)
				{
					Bitmap mewbm = NGetBitmap(data.Data);

					Bitmap newBitmap = scaleDown(mewbm, 200, true);
					using (var stream = new MemoryStream())
					{
						newBitmap.Compress(Bitmap.CompressFormat.Png, 0, stream);
						bitmapByteData = stream.ToArray();
					}
					ExportBitmapAsPNG(GetRoundedCornerBitmap(newBitmap, 400));
				}
			}
			catch (Exception err)
			{
				Toast.MakeText(Activity, err.ToString(), ToastLength.Long).Show();
			}
        }

        void ExportBitmapAsPNG(Bitmap bitmap)
        {
            try
            {
                var sdCardPath = Android.OS.Environment.DataDirectory.AbsolutePath;
                var filePath = System.IO.Path.Combine(sdCardPath, "data/goheja.gohejanitro/files/me.png");
                var stream = new FileStream(filePath, FileMode.Create);

                bitmap.Compress(Bitmap.CompressFormat.Png, 100, stream);// Bitmap.CompressFormat.Png, 100, stream);
                stream.Close();
                var s2 = new FileStream(filePath, FileMode.Open);

                Bitmap bitmap2 = BitmapFactory.DecodeFile(filePath);
                meImage.SetImageBitmap(bitmap2);
                s2.Close();
                GC.Collect();
            }
            catch (Exception err)
            {
                Toast.MakeText(Activity, err.ToString(), ToastLength.Long).Show();
            }
        }
        private Bitmap NGetBitmap(Android.Net.Uri uriImage)
        {
            Bitmap mBitmap = null;
            mBitmap = MediaStore.Images.Media.GetBitmap(Activity.ContentResolver, uriImage);
            return mBitmap;
        }
        private static Bitmap GetRoundedCornerBitmap(Bitmap bitmap, int pixels)
        {
            Bitmap output = null;

            try
            {
                output = Bitmap.CreateBitmap(bitmap.Width, bitmap.Height, Bitmap.Config.Argb8888);
                Canvas canvas = new Canvas(output);

                Color color = new Color(66, 66, 66);
                Paint paint = new Paint();
                Rect rect = new Rect(0, 0, bitmap.Width * 5 / 5, bitmap.Height * 5 / 5);
                RectF rectF = new RectF(rect);
                float roundPx = pixels;

                paint.AntiAlias = true;
                canvas.DrawARGB(0, 0, 0, 0);
                paint.Color = color;
                canvas.DrawRoundRect(rectF, roundPx, roundPx, paint);

                paint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.SrcIn));
                canvas.DrawBitmap(bitmap, rect, rect, paint);
            }
            catch
            {
				return null;
            }

            return output;
        }

        public static Bitmap scaleDown(Bitmap realImage, float maxImageSize, bool filter)
        {
            float ratio = Math.Min((float)maxImageSize / realImage.Width, (float)maxImageSize / realImage.Width);
            int width = (int)Math.Round((float)ratio * realImage.Width);
            int height = (int)Math.Round((float)ratio * realImage.Width);

            Bitmap newBitmap = Bitmap.CreateScaledBitmap(realImage, width, height, filter);
            return newBitmap;
        }
        void setBitmapImg()
        {
            try
            {
                var sdCardPath = Android.OS.Environment.DataDirectory.AbsolutePath;
                var filePath = System.IO.Path.Combine(sdCardPath, "data/goheja.gohejanitro/files/me.png");
                var s2 = new FileStream(filePath, FileMode.Open);
                Bitmap bitmap2 = BitmapFactory.DecodeFile(filePath);
                meImage.SetImageBitmap(bitmap2);
                s2.Close();
            }
            catch
            {
            }
            finally
            {
                GC.Collect();
            }
        }
        private void setPersonalData()
        {
			try
			{
				trackSvc.Service1 sv = new trackSvc.Service1();
				string _dId = Settings.Secure.GetString(Activity.ContentResolver, Settings.Secure.AndroidId);
				string[] athData = sv.getAthDataByDeviceId(_dId);
				lblUsername.Text = athData[4];
				tvFirstName.Text = athData[0];
				tvLastName.Text = athData[1];
				tvEmail.Text = athData[6];
				tvPhone.Text = athData[7];
			}
			catch
			{
				Toast.MakeText(Activity, "Error getting user data!", ToastLength.Long).Show();
			}
        }
    }
}
