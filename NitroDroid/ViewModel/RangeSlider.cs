﻿using System;
using System.Globalization;
using System.Threading.Tasks;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using Xamarin.RangeSlider.Common;

namespace goheja
{
	public class RangeSliderControl : ImageView
	{
		public static readonly Color DefaultActiveColor = Color.Argb(0xFF, 0x33, 0xB5, 0xE5);

		/// <summary>
		///     An invalid pointer id.
		/// </summary>
		public const int InvalidPointerId = 255;

		// Localized constants from MotionEvent for compatibility
		// with API < 8 "Froyo".
		public const int ActionPointerIndexMask = 0x0000ff00, ActionPointerIndexShift = 8;

		public const int DefaultMinimum = 0;
		public const int DefaultMaximum = 100;
		public const int HeightInDp = 16;
		public const int TextLateralPaddingInDp = 3;

		private const int InitialPaddingInDp = 8;
		private const int DefaultTextSizeInSp = 15;
		private const int DefaultTextDistanceToButtonInDp = 8;
		private const int DefaultTextDistanceToTopInDp = 8;

		private const int DefaultStepValue = 0;

		private const int LineHeightInDp = 1;
		private readonly Paint _paint = new Paint(PaintFlags.AntiAlias);
		private readonly Paint _shadowPaint = new Paint();
		private readonly Matrix _thumbShadowMatrix = new Matrix();
		private readonly Path _translatedThumbShadowPath = new Path();

		private int _activePointerId = InvalidPointerId;
		private int _distanceToTop;

		private float _downMotionX;
		private float _internalPad;

		private bool _isDragging;

		private float _padding;
		private Thumb? _pressedThumb;
		private RectF _rect;

		private int _scaledTouchSlop;

		private int _textOffset;
		private int _textSize;
		private float _thumbHalfHeight;

		private float _thumbHalfWidth;
		private int _thumbShadowBlur;
		private Path _thumbShadowPath;
		protected float AbsoluteMinValue, AbsoluteMaxValue;
		protected float MinDeltaForDefault = 0;
		protected float NormalizedMaxValue = 1f;
		protected float NormalizedMinValue;
		private Color _activeColor;
		private bool _minThumbHidden;
		private bool _maxThumbHidden;
		private bool _showTextAboveThumbs;
		private float _barHeight;
		private string _textFormat = "F0";

		private float MinToMaxRange
		{
			get
			{
				return AbsoluteMaxValue - AbsoluteMinValue;
			}
		}

		protected RangeSliderControl(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
		{
		}

		public RangeSliderControl(Context context) : base(context)
		{
			Init(context, null);
		}

		public RangeSliderControl(Context context, IAttributeSet attrs) : base(context, attrs)
		{
			Init(context, attrs);
		}

		public RangeSliderControl(Context context, IAttributeSet attrs, int defStyleAttr)
			: base(context, attrs, defStyleAttr)
		{
			Init(context, attrs);
		}

		public RangeSliderControl(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes)
			: base(context, attrs, defStyleAttr, defStyleRes)
		{
			Init(context, attrs);
		}

		public bool ActivateOnDefaultValues { get; set; }

		public Color ActiveColor
		{
			get { return _activeColor; }
			set
			{
				_activeColor = value;
				Invalidate();
			}
		}

		public Func<Thumb, float, string> FormatLabel { get; set; }

		public override bool Enabled
		{
			get
			{
				return base.Enabled;
			}
			set
			{
				base.Enabled = value;
				Invalidate();
			}
		}

		public bool AlwaysActive { get; set; }
		public Color DefaultColor { get; set; }
		public bool ShowLabels { get; set; }

		public int TextSizeInSp
		{
			get { return PixelUtil.PxToSp(Context, _textSize); }
			set
			{
				_textSize = PixelUtil.SpToPx(Context, value);
				UpdateTextOffset();
				SetBarHeight(_barHeight);
				RequestLayout();
				Invalidate();
			}
		}

		public bool ShowTextAboveThumbs
		{
			get { return _showTextAboveThumbs; }
			set
			{
				_showTextAboveThumbs = value;
				UpdateTextOffset();
				SetBarHeight(_barHeight);
				RequestLayout();
				Invalidate();
			}
		}

		public string TextFormat
		{
			get { return _textFormat; }
			set
			{
				_textFormat = string.IsNullOrWhiteSpace(value) ? "F0" : value;
				Invalidate();
			}
		}

		public bool MinThumbHidden
		{
			get { return _minThumbHidden; }
			set
			{
				_minThumbHidden = value;
				Invalidate();
			}
		}

		public bool MaxThumbHidden
		{
			get { return _maxThumbHidden; }
			set
			{
				_maxThumbHidden = value;
				Invalidate();
			}
		}

		public Color TextAboveThumbsColor { get; set; }
		public Bitmap ThumbDisabledImage { get; set; }

		public Bitmap ThumbImage { get; set; }
		public Bitmap ThumbPressedImage { get; set; }

		public bool ThumbShadow { get; set; }
		public int ThumbShadowXOffset { get; set; }
		public int ThumbShadowYOffset { get; set; }

		/// <summary>
		///     Should the widget notify the listener callback while the user is still dragging a thumb? Default is false.
		/// </summary>
		public bool NotifyWhileDragging { get; set; }

		/// <summary>
		///     default 0.0 (disabled)
		/// </summary>
		public float StepValue { get; set; }

		/// <summary>
		/// If false the slider will move freely with the tounch. When the touch ends, the value will snap to the nearest step value
		/// If true the slider will stay in its current position until it reaches a new step value.
		/// default false
		/// </summary>
		public bool StepValueContinuously { get; set; }

		private float ExtractNumericValueFromAttributes(TypedArray a, int attribute, int defaultValue)
		{
			var tv = a.PeekValue(attribute);
			return tv == null ? defaultValue : a.GetFloat(attribute, defaultValue);
		}

		private void Init(Context context, IAttributeSet attrs)
		{
			var thumbNormal = Resource.Drawable.icon_seek_thumb_act;
			var thumbPressed = Resource.Drawable.icon_seek_thumb_dis;
			var thumbDisabled = Resource.Drawable.icon_seek_thumb_act;
			Color thumbShadowColor;
			var defaultShadowColor = Color.Argb(75, 0, 0, 0);
			var defaultShadowYOffset = PixelUtil.DpToPx(context, 2);
			var defaultShadowXOffset = PixelUtil.DpToPx(context, 0);
			var defaultShadowBlur = PixelUtil.DpToPx(context, 2);

			_distanceToTop = PixelUtil.DpToPx(context, DefaultTextDistanceToTopInDp);

			if (attrs == null)
			{
				SetRangeToDefaultValues();
				_internalPad = PixelUtil.DpToPx(context, InitialPaddingInDp);
				_barHeight = PixelUtil.DpToPx(context, LineHeightInDp);
				ActiveColor = DefaultActiveColor;
				DefaultColor = Color.Gray;
				AlwaysActive = false;
				ShowTextAboveThumbs = true;
				TextAboveThumbsColor = Color.White;
				thumbShadowColor = defaultShadowColor;
				ThumbShadowXOffset = defaultShadowXOffset;
				ThumbShadowYOffset = defaultShadowYOffset;
				_thumbShadowBlur = defaultShadowBlur;
				ActivateOnDefaultValues = false;
				TextSizeInSp = DefaultTextSizeInSp;
			}
			else
			{
				var a = Context.ObtainStyledAttributes(attrs, Resource.Styleable.RangeSliderControl, 0, 0);
				try
				{
					SetRangeValues(ExtractNumericValueFromAttributes(a, Resource.Styleable.RangeSliderControl_absoluteMinValue, DefaultMinimum),
						ExtractNumericValueFromAttributes(a, Resource.Styleable.RangeSliderControl_absoluteMaxValue, DefaultMaximum));
					ShowTextAboveThumbs = a.GetBoolean(Resource.Styleable.RangeSliderControl_valuesAboveThumbs, true);
					TextAboveThumbsColor = a.GetColor(Resource.Styleable.RangeSliderControl_textAboveThumbsColor, Color.White);
					MinThumbHidden = a.GetBoolean(Resource.Styleable.RangeSliderControl_minThumbHidden, false);
					MaxThumbHidden = a.GetBoolean(Resource.Styleable.RangeSliderControl_maxThumbHidden, false);
					ShowLabels = a.GetBoolean(Resource.Styleable.RangeSliderControl_showRangeLabels, true);
					_internalPad = a.GetDimensionPixelSize(Resource.Styleable.RangeSliderControl_internalPadding, InitialPaddingInDp);
					_barHeight = a.GetDimensionPixelSize(Resource.Styleable.RangeSliderControl_barHeight, LineHeightInDp);
					ActiveColor = a.GetColor(Resource.Styleable.RangeSliderControl_activeColor, DefaultActiveColor);
					DefaultColor = a.GetColor(Resource.Styleable.RangeSliderControl_defaultColor, Color.Gray);
					AlwaysActive = a.GetBoolean(Resource.Styleable.RangeSliderControl_alwaysActive, false);
					StepValue = ExtractNumericValueFromAttributes(a,
						Resource.Styleable.RangeSliderControl_stepValue, DefaultStepValue);
					StepValueContinuously = a.GetBoolean(Resource.Styleable.RangeSliderControl_stepValueContinuously,
						false);

					var normalDrawable = a.GetDrawable(Resource.Styleable.RangeSliderControl_thumbNormal);
					if (normalDrawable != null)
					{
						ThumbImage = BitmapUtil.DrawableToBitmap(normalDrawable);
					}
					var disabledDrawable = a.GetDrawable(Resource.Styleable.RangeSliderControl_thumbDisabled);
					if (disabledDrawable != null)
					{
						ThumbDisabledImage = BitmapUtil.DrawableToBitmap(disabledDrawable);
					}
					var pressedDrawable = a.GetDrawable(Resource.Styleable.RangeSliderControl_thumbPressed);
					if (pressedDrawable != null)
					{
						ThumbPressedImage = BitmapUtil.DrawableToBitmap(pressedDrawable);
					}
					ThumbShadow = a.GetBoolean(Resource.Styleable.RangeSliderControl_thumbShadow, false);
					thumbShadowColor = a.GetColor(Resource.Styleable.RangeSliderControl_thumbShadowColor, defaultShadowColor);
					ThumbShadowXOffset = a.GetDimensionPixelSize(Resource.Styleable.RangeSliderControl_thumbShadowXOffset, defaultShadowXOffset);
					ThumbShadowYOffset = a.GetDimensionPixelSize(Resource.Styleable.RangeSliderControl_thumbShadowYOffset, defaultShadowYOffset);
					_thumbShadowBlur = a.GetDimensionPixelSize(Resource.Styleable.RangeSliderControl_thumbShadowBlur, defaultShadowBlur);

					ActivateOnDefaultValues = a.GetBoolean(Resource.Styleable.RangeSliderControl_activateOnDefaultValues, false);
					TextSizeInSp = a.GetInt(Resource.Styleable.RangeSliderControl_textSize, DefaultTextSizeInSp);
				}
				finally
				{
					a.Recycle();
				}
			}

			if (ThumbImage == null)
			{
				ThumbImage = BaseActivity.ScaleDownImg(BitmapFactory.DecodeResource(Resources, thumbNormal), 50, true);
			}
			if (ThumbPressedImage == null)
			{
				ThumbPressedImage = BaseActivity.ScaleDownImg(BitmapFactory.DecodeResource(Resources, thumbPressed), 50, true);
			}
			if (ThumbDisabledImage == null)
			{
				ThumbDisabledImage = BaseActivity.ScaleDownImg(BitmapFactory.DecodeResource(Resources, thumbDisabled), 50, true);
			}

			_thumbHalfWidth = 0.5f * ThumbImage.Width;
			_thumbHalfHeight = 0.5f * ThumbImage.Height;

			SetBarHeight(_barHeight);

			// make RangeSliderControl focusable. This solves focus handling issues in case EditText widgets are being used along with the RangeSliderControl within ScrollViews.
			Focusable = true;
			FocusableInTouchMode = true;
			_scaledTouchSlop = ViewConfiguration.Get(Context).ScaledTouchSlop;

			if (ThumbShadow)
			{
				// We need to remove hardware acceleration in order to blur the shadow
				SetLayerType(LayerType.Software, null);
				_shadowPaint.Color = thumbShadowColor;
				_shadowPaint.SetMaskFilter(new BlurMaskFilter(_thumbShadowBlur, BlurMaskFilter.Blur.Normal));
				_thumbShadowPath = new Path();
				_thumbShadowPath.AddCircle(0, 0, _thumbHalfHeight, Path.Direction.Cw);
			}
		}

		public void SetBarHeight(float barHeight)
		{
			_barHeight = barHeight;
			if (_rect == null)
				_rect = new RectF(_padding,
					_textOffset + _thumbHalfHeight - barHeight / 2,
					Width - _padding,
					_textOffset + _thumbHalfHeight + barHeight / 2);
			else
				_rect = new RectF(_rect.Left,
					_textOffset + _thumbHalfHeight - barHeight / 2,
					_rect.Right,
					_textOffset + _thumbHalfHeight + barHeight / 2);
			Invalidate();
		}

		public void SetRangeValues(float minValue, float maxValue)
		{
			var oldMinValue = NormalizedToValue(NormalizedMinValue);
			var oldMaxValue = NormalizedToValue(NormalizedMaxValue);
			AbsoluteMinValue = minValue;
			AbsoluteMaxValue = maxValue;
			if (Math.Abs(MinToMaxRange) < float.Epsilon)
			{
				SetNormalizedMinValue(0f, true, true);
				SetNormalizedMaxValue(0f, true, true);
			}
			else
			{
				SetNormalizedMinValue(ValueToNormalized(oldMinValue), true, true);
				SetNormalizedMaxValue(ValueToNormalized(oldMaxValue), true, true);
			}
			Invalidate();
		}

		public void SetTextAboveThumbsColor(Color textAboveThumbsColor)
		{
			TextAboveThumbsColor = textAboveThumbsColor;
			Invalidate();
		}

		public void SetTextAboveThumbsColorResource(int resId)
		{
			SetTextAboveThumbsColor(new Color(ContextCompat.GetColor(Context, resId)));
		}


		/// <summary>
		/// only used to set default values when initialised from XML without any values specified
		/// </summary>
		private void SetRangeToDefaultValues()
		{
			AbsoluteMinValue = DefaultMinimum;
			AbsoluteMaxValue = DefaultMaximum;
		}

		public void ResetSelectedValues()
		{
			SetSelectedMinValue(AbsoluteMinValue);
			SetSelectedMaxValue(AbsoluteMaxValue);
		}

		/// <summary>
		/// Returns the absolute minimum value of the range that has been set at construction time.
		/// </summary>
		/// <returns>The absolute minimum value of the range.</returns>
		public float GetAbsoluteMinValue()
		{
			return AbsoluteMinValue;
		}

		/// <summary>
		/// Returns the absolute maximum value of the range that has been set at construction time.
		/// </summary>
		/// <returns>The absolute maximum value of the range.</returns>
		public float GetAbsoluteMaxValue()
		{
			return AbsoluteMaxValue;
		}

		/// <summary>
		/// Returns the currently selected min value.
		/// </summary>
		/// <returns>The currently selected min value.</returns>
		public float GetSelectedMinValue()
		{
			return NormalizedToValue(NormalizedMinValue);
		}

		/// <summary>
		/// Sets the currently selected minimum value. The widget will be Invalidated and redrawn.
		/// </summary>
		/// <param name="value">The Number value to set the minimum value to. Will be clamped to given absolute minimum/maximum range.</param>
		public void SetSelectedMinValue(float value)
		{
			if (_pressedThumb == Thumb.Lower)
				return;
			// in case absoluteMinValue == absoluteMaxValue, avoid division by zero when normalizing.
			SetNormalizedMinValue(Math.Abs(MinToMaxRange) < float.Epsilon
				? 0f
				: ValueToNormalized(value), true, false);
		}

		/// <summary>
		/// Returns the currently selected max value.
		/// </summary>
		/// <returns>The currently selected max value.</returns>
		public float GetSelectedMaxValue()
		{
			return NormalizedToValue(NormalizedMaxValue);
		}

		/// <summary>
		/// Sets the currently selected maximum value. The widget will be Invalidated and redrawn.
		/// </summary>
		/// <param name="value">The Number value to set the maximum value to. Will be clamped to given absolute minimum/maximum range.</param>
		public void SetSelectedMaxValue(float value)
		{
			if (_pressedThumb == Thumb.Upper)
				return;
			// in case absoluteMinValue == absoluteMaxValue, avoid division by zero when normalizing.
			SetNormalizedMaxValue(Math.Abs(MinToMaxRange) < float.Epsilon
				? 1f
				: ValueToNormalized(value), true, false);
		}

		/// <summary>
		/// Set the path that defines the shadow of the thumb. This path should be defined assuming
		/// that the center of the shadow is at the top left corner(0,0) of the canvas.The
		/// <see cref="DrawThumbShadow"/> method will place the shadow appropriately.
		/// </summary>
		/// <param name="thumbShadowPath">The path defining the thumb shadow</param>
		public void SetThumbShadowPath(Path thumbShadowPath)
		{
			_thumbShadowPath = thumbShadowPath;
		}

		/// <summary>
		///     Handles thumb selection and movement. Notifies listener callback on certain evs.
		/// </summary>
		public override bool OnTouchEvent(MotionEvent ev)
		{
			if (!Enabled)
			{
				return false;
			}

			int pointerIndex;

			var action = ev.Action;
			switch (action & MotionEventActions.Mask)
			{
				case MotionEventActions.Down:
					// Remember where the motion ev started
					_activePointerId = ev.GetPointerId(ev.PointerCount - 1);
					pointerIndex = ev.FindPointerIndex(_activePointerId);
					_downMotionX = ev.GetX(pointerIndex);

					_pressedThumb = EvalPressedThumb(_downMotionX);

					// Only handle thumb presses.
					if (_pressedThumb == null)
					{
						return base.OnTouchEvent(ev);
					}

					Pressed = true;
					Invalidate();
					OnStartTrackingTouch();
					TrackTouchEvent(ev, StepValueContinuously);
					AttemptClaimDrag();

					break;
				case MotionEventActions.Move:
					if (_pressedThumb != null)
					{
						if (_isDragging)
						{
							TrackTouchEvent(ev, StepValueContinuously);
						}
						else
						{
							// Scroll to follow the motion ev
							pointerIndex = ev.FindPointerIndex(_activePointerId);
							var x = ev.GetX(pointerIndex);

							if (Math.Abs(x - _downMotionX) > _scaledTouchSlop)
							{
								Pressed = true;
								Invalidate();
								OnStartTrackingTouch();
								TrackTouchEvent(ev, StepValueContinuously);
								AttemptClaimDrag();
							}
						}

						if (NotifyWhileDragging)
						{
							if (_pressedThumb == Thumb.Lower)
								OnLowerValueChanged();
							if (_pressedThumb == Thumb.Upper)
								OnUpperValueChanged();
						}
					}
					break;
				case MotionEventActions.Up:
					if (_isDragging)
					{
						TrackTouchEvent(ev, true);
						OnStopTrackingTouch();
						Pressed = false;
					}
					else
					{
						// Touch up when we never crossed the touch slop threshold
						// should be interpreted as a tap-seek to that location.
						OnStartTrackingTouch();
						TrackTouchEvent(ev, true);
						OnStopTrackingTouch();
					}
					if (_pressedThumb == Thumb.Lower)
						OnLowerValueChanged();
					if (_pressedThumb == Thumb.Upper)
						OnUpperValueChanged();
					_pressedThumb = null;
					Invalidate();
					break;
				case MotionEventActions.PointerDown:
					var index = ev.PointerCount - 1;
					// readonly int index = ev.getActionIndex();
					_downMotionX = ev.GetX(index);
					_activePointerId = ev.GetPointerId(index);
					Invalidate();
					break;
				case MotionEventActions.PointerUp:
					OnSecondaryPointerUp(ev);
					Invalidate();
					break;
				case MotionEventActions.Cancel:
					if (_isDragging)
					{
						OnStopTrackingTouch();
						Pressed = false;
					}
					Invalidate(); // see above explanation
					break;
			}
			return true;
		}

		private void OnSecondaryPointerUp(MotionEvent ev)
		{
			var pointerIndex = (int)(ev.Action & MotionEventActions.PointerIndexMask) >>
							   (int)MotionEventActions.PointerIndexShift;

			var pointerId = ev.GetPointerId(pointerIndex);
			if (pointerId == _activePointerId)
			{
				// This was our active pointer going up. Choose
				// a new active pointer and adjust accordingly.
				// TODO: Make this decision more intelligent.
				var newPointerIndex = pointerIndex == 0 ? 1 : 0;
				_downMotionX = ev.GetX(newPointerIndex);
				_activePointerId = ev.GetPointerId(newPointerIndex);
			}
		}

		private void TrackTouchEvent(MotionEvent ev, bool step)
		{
			var pointerIndex = ev.FindPointerIndex(_activePointerId);
			var x = ev.GetX(pointerIndex);

			if (Thumb.Lower.Equals(_pressedThumb) && !MinThumbHidden)
			{
				SetNormalizedMinValue(ScreenToNormalized(x), step, true);
			}
			else if (Thumb.Upper.Equals(_pressedThumb) && !MaxThumbHidden)
			{
				SetNormalizedMaxValue(ScreenToNormalized(x), step, true);
			}
		}

		/// <summary>
		///     Tries to claim the user's drag motion, and requests disallowing any ancestors from stealing evs in the drag.
		/// </summary>
		private void AttemptClaimDrag()
		{
			Parent?.RequestDisallowInterceptTouchEvent(true);
		}

		/// <summary>
		///     This is called when the user has started touching this widget.
		/// </summary>
		private void OnStartTrackingTouch()
		{
			_isDragging = true;
			DragStarted?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		///     This is called when the user either releases his touch or the touch is canceled.
		/// </summary>
		private void OnStopTrackingTouch()
		{
			_isDragging = false;
			DragCompleted?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		///     Ensures correct size of the widget.
		/// </summary>
		protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
		{
			var width = 200;
			if (MeasureSpecMode.Unspecified != MeasureSpec.GetMode(widthMeasureSpec))
			{
				width = MeasureSpec.GetSize(widthMeasureSpec);
			}

			var height = ThumbImage.Height
						 + (ShowTextAboveThumbs ? PixelUtil.DpToPx(Context, HeightInDp) + PixelUtil.SpToPx(Context, TextSizeInSp) : 0)
						 + (ThumbShadow ? ThumbShadowYOffset + _thumbShadowBlur : 0);
			if (MeasureSpecMode.Unspecified != MeasureSpec.GetMode(heightMeasureSpec))
			{
				height = Math.Min(height, MeasureSpec.GetSize(heightMeasureSpec));
			}
			SetMeasuredDimension(width, height);
		}

		/// <summary>
		///     Draws the widget on the given canvas.
		/// </summary>
		protected async override void OnDraw(Canvas canvas)
		{
			base.OnDraw(canvas);

			_paint.TextSize = _textSize;
			_paint.SetStyle(Paint.Style.Fill);
			_paint.Color = DefaultColor;
			_paint.AntiAlias = true;
			float minMaxLabelSize = 0;

			if (ShowLabels)
			{
				// draw min and max labels
				var minLabel = Context.GetString(Resource.String.demo_min_label);
				var maxLabel = Context.GetString(Resource.String.demo_max_label);
				minMaxLabelSize = Math.Max(_paint.MeasureText(minLabel), _paint.MeasureText(maxLabel));
				var minMaxHeight = _textOffset + _thumbHalfHeight + (float)_textSize / 3;
				canvas.DrawText(minLabel, 0, minMaxHeight, _paint);
				canvas.DrawText(maxLabel, Width - minMaxLabelSize, minMaxHeight, _paint);
			}
			_padding = _internalPad + minMaxLabelSize + _thumbHalfWidth;

			// draw seek bar background line
			//await Task.Delay(500);
			_rect.Left = _padding;
			_rect.Right = Width - _padding;
			canvas.DrawRect(_rect, _paint);

			var selectedValuesAreDefault = NormalizedMinValue <= MinDeltaForDefault &&
										   NormalizedMaxValue >= 1 - MinDeltaForDefault;

			var colorToUseForButtonsAndHighlightedLine =
				!Enabled || (!AlwaysActive && !ActivateOnDefaultValues && selectedValuesAreDefault)
					? DefaultColor // default values
					: ActiveColor; // non default, filter is active

			// draw seek bar active range line
			_rect.Left = NormalizedToScreen(NormalizedMinValue);
			_rect.Right = NormalizedToScreen(NormalizedMaxValue);

			_paint.Color = colorToUseForButtonsAndHighlightedLine;
			canvas.DrawRect(_rect, _paint);

			// draw minimum thumb (& shadow if requested) if not a single thumb control
			if (!MinThumbHidden)
			{
				if (ThumbShadow)
				{
					DrawThumbShadow(NormalizedToScreen(NormalizedMinValue), canvas);
				}
				DrawThumb(NormalizedToScreen(NormalizedMinValue), Thumb.Lower.Equals(_pressedThumb), canvas,
					selectedValuesAreDefault);
			}

			// draw maximum thumb & shadow (if necessary)
			if (!MaxThumbHidden)
			{
				if (ThumbShadow)
				{
					DrawThumbShadow(NormalizedToScreen(NormalizedMaxValue), canvas);
				}
				DrawThumb(NormalizedToScreen(NormalizedMaxValue), Thumb.Upper.Equals(_pressedThumb), canvas,
					selectedValuesAreDefault);
			}

			// draw the text if sliders have moved from default edges
			if (!ShowTextAboveThumbs || (!ActivateOnDefaultValues && selectedValuesAreDefault))
				return;

			_paint.TextSize = _textSize;
			_paint.Color = TextAboveThumbsColor;

			var minText = ValueToString(GetSelectedMinValue(), Thumb.Lower);
			var maxText = ValueToString(GetSelectedMaxValue(), Thumb.Upper);
			var minTextWidth = _paint.MeasureText(minText);
			var maxTextWidth = _paint.MeasureText(maxText);
			// keep the position so that the labels don't get cut off
			var minPosition = Math.Max(0f, NormalizedToScreen(NormalizedMinValue) - minTextWidth * 0.5f);
			var maxPosition = Math.Min(Width - maxTextWidth,
				NormalizedToScreen(NormalizedMaxValue) - maxTextWidth * 0.5f);

			if (!MaxThumbHidden && !MinThumbHidden)
			{
				// check if the labels overlap, or are too close to each other
				var spacing = PixelUtil.DpToPx(Context, TextLateralPaddingInDp);
				var overlap = minPosition + minTextWidth - maxPosition + spacing;
				if (overlap > 0f)
				{
					// we could move them the same ("overlap * 0.5f")
					// but we rather move more the one which is farther from the ends, as it has more space
					minPosition -= overlap * NormalizedMinValue / (NormalizedMinValue + 1 - NormalizedMaxValue);
					maxPosition += overlap * (1 - NormalizedMaxValue) / (NormalizedMinValue + 1 - NormalizedMaxValue);
				}
				canvas.DrawText(minText,
					minPosition,
					_distanceToTop + _textSize,
					_paint);
			}

			canvas.DrawText(maxText,
				maxPosition,
				_distanceToTop + _textSize,
				_paint);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			_rect.Dispose();
			GC.Collect();
		}
		protected string ValueToString(float value, Thumb thumb)
		{
			var func = FormatLabel;
			return func == null
				? value.ToString(_textFormat, CultureInfo.InvariantCulture)
				: func(thumb, value);
		}

		/// <summary>
		///     Overridden to save instance state when device orientation changes. This method is called automatically if you
		///     assign an id to the RangeSliderControl widget using the Id. Other members of this class than the normalized min and
		///     max values don't need to be saved.
		/// </summary>
		protected override IParcelable OnSaveInstanceState()
		{
			var bundle = new Bundle();
			bundle.PutParcelable("SUPER", base.OnSaveInstanceState());
			bundle.PutDouble("MIN", NormalizedMinValue);
			bundle.PutDouble("MAX", NormalizedMaxValue);
			return bundle;
		}

		/// <summary>
		///     Overridden to restore instance state when device orientation changes. This method is called automatically if you
		///     assign an id to the RangeSliderControl widget using the {@link #setId(int)} method.
		/// </summary>
		protected override void OnRestoreInstanceState(IParcelable parcel)
		{
			var bundle = (Bundle)parcel;
			base.OnRestoreInstanceState((IParcelable)bundle.GetParcelable("SUPER"));
			NormalizedMinValue = bundle.GetFloat("MIN");
			NormalizedMaxValue = bundle.GetFloat("MAX");
		}

		/// <summary>
		///     Draws the "normal" resp. "pressed" thumb image on specified x-coordinate.
		/// </summary>
		/// <param name="screenCoord">The x-coordinate in screen space where to draw the image.</param>
		/// <param name="pressed">Is the thumb currently in "pressed" state?</param>
		/// <param name="canvas">The canvas to draw upon.</param>
		/// <param name="areSelectedValuesDefault"></param>
		private void DrawThumb(float screenCoord, bool pressed, Canvas canvas, bool areSelectedValuesDefault)
		{
			Bitmap buttonToDraw;
			if (!Enabled || (!ActivateOnDefaultValues && areSelectedValuesDefault))
				buttonToDraw = ThumbDisabledImage;
			else
				buttonToDraw = pressed ? ThumbPressedImage : ThumbImage;

			canvas.DrawBitmap(buttonToDraw, screenCoord - _thumbHalfWidth, _textOffset, _paint);
		}

		/// <summary>
		///     Draws a drop shadow beneath the slider thumb.
		/// </summary>
		/// <param name="screenCoord">the x-coordinate of the slider thumb</param>
		/// <param name="canvas">the canvas on which to draw the shadow</param>
		private void DrawThumbShadow(float screenCoord, Canvas canvas)
		{
			_thumbShadowMatrix.SetTranslate(screenCoord + ThumbShadowXOffset,
				_textOffset + _thumbHalfHeight + ThumbShadowYOffset);
			_translatedThumbShadowPath.Set(_thumbShadowPath);
			_translatedThumbShadowPath.Transform(_thumbShadowMatrix);
			canvas.DrawPath(_translatedThumbShadowPath, _shadowPaint);
		}

		/// <summary>
		/// Decides which (if any) thumb is touched by the given x-coordinate.
		/// </summary>
		/// <param name="touchX">The x-coordinate of a touch ev in screen space.</param>
		/// <returns>The pressed thumb or null if none has been touched.</returns>
		private Thumb? EvalPressedThumb(float touchX)
		{
			Thumb? result = null;
			var minThumbPressed = IsInThumbRange(touchX, NormalizedMinValue);
			var maxThumbPressed = IsInThumbRange(touchX, NormalizedMaxValue);
			if (minThumbPressed && maxThumbPressed)
				// if both thumbs are pressed (they lie on top of each other), choose the one with more room to drag. this avoids "stalling" the thumbs in a corner, not being able to drag them apart anymore.
				result = touchX / Width > 0.5f ? Thumb.Lower : Thumb.Upper;
			else if (minThumbPressed)
				result = Thumb.Lower;
			else if (maxThumbPressed)
				result = Thumb.Upper;
			return result;
		}

		/// <summary>
		///     Decides if given x-coordinate in screen space needs to be interpreted as "within" the normalized thumb
		///     x-coordinate.
		/// </summary>
		/// <param name="touchX">The x-coordinate in screen space to check.</param>
		/// <param name="normalizedThumbValue">The normalized x-coordinate of the thumb to check.</param>
		/// <returns>true if x-coordinate is in thumb range, false otherwise.</returns>
		private bool IsInThumbRange(float touchX, float normalizedThumbValue)
		{
			return Math.Abs(touchX - NormalizedToScreen(normalizedThumbValue)) <= _thumbHalfWidth;
		}

		/// <summary>
		/// Sets normalized min value to value so that 0 <= value <= normalized max value <= 1. The View will get Invalidated when calling this method.
		/// </summary>
		/// <param name="value">The new normalized min value to set.</param>
		/// <param name="step">If true then value is rounded to <see cref="StepValue"/></param>
		/// <param name="checkValue">If true check if value falls inside min/max</param>
		private void SetNormalizedMinValue(float value, bool step, bool checkValue)
		{
			NormalizedMinValue = checkValue
									? Math.Max(0f, Math.Min(1f, Math.Min(value, NormalizedMaxValue)))
									: value;
			if (step)
				NormalizedMinValue = ValueToNormalized(NormalizedToValue(NormalizedMinValue));
			Invalidate();
		}

		/// <summary>
		/// Sets normalized max value to value so that 0 <= normalized min value <= value <= 1. The View will get Invalidated when calling this method.
		/// </summary>
		/// <param name="value">The new normalized max value to set.</param>
		/// <param name="step">If true then value is rounded to <see cref="StepValue"/></param>
		/// <param name="checkValue">If true check if value falls inside min/max</param>
		private void SetNormalizedMaxValue(float value, bool step, bool checkValue)
		{
			NormalizedMaxValue = checkValue
									? Math.Max(0f, Math.Min(1f, Math.Max(value, NormalizedMinValue)))
									: value;
			if (step)
				NormalizedMaxValue = ValueToNormalized(NormalizedToValue(NormalizedMaxValue));
			Invalidate();
		}

		/// <summary>
		///     Converts a normalized value to a Number object in the value space between absolute minimum and maximum.
		/// </summary>
		protected float NormalizedToValue(float normalized)
		{
			var v = AbsoluteMinValue + normalized * MinToMaxRange;
			// TODO parameterize this rounding to allow variable decimal points
			if (Math.Abs(StepValue) < float.Epsilon)
				return (float)Math.Round(v * 100) / 100f;
			var normalizedToValue = (float)Math.Round(v / StepValue) * StepValue;
			return Math.Max(AbsoluteMinValue, Math.Min(AbsoluteMaxValue, normalizedToValue));
		}

		/// <summary>
		/// Converts the given Number value to a normalized float.
		/// </summary>
		/// <param name="value">The Number value to normalize.</param>
		/// <returns>The normalized float.</returns>
		protected float ValueToNormalized(float value)
		{
			if (Math.Abs(MinToMaxRange) < float.Epsilon)
			{
				// prev division by zero, simply return 0.
				return 0f;
			}
			return (value - AbsoluteMinValue) / MinToMaxRange;
		}

		private void UpdateTextOffset()
		{
			_textOffset = _showTextAboveThumbs
				? _textSize + PixelUtil.DpToPx(Context, DefaultTextDistanceToButtonInDp) + _distanceToTop
				: 0;
		}


		/// <summary>
		/// Converts a normalized value into screen space.
		/// </summary>
		/// <param name="normalizedCoord">The normalized value to convert.</param>
		/// <returns>The converted value in screen space.</returns>
		private float NormalizedToScreen(float normalizedCoord)
		{
			return _padding + normalizedCoord * (Width - 2 * _padding);
		}

		/// <summary>
		/// Converts screen space x-coordinates into normalized values.
		/// </summary>
		/// <param name="screenCoord">The x-coordinate in screen space to convert.</param>
		/// <returns>The normalized value.</returns>
		private float ScreenToNormalized(float screenCoord)
		{
			var width = Width;
			if (width <= 2 * _padding)
			{
				// prev division by zero, simply return 0.
				return 0f;
			}
			var result = (screenCoord - _padding) / (width - 2 * _padding);
			return Math.Min(1f, Math.Max(0f, result));
		}

		public event EventHandler LowerValueChanged;
		public event EventHandler UpperValueChanged;
		public event EventHandler DragStarted;
		public event EventHandler DragCompleted;

		protected virtual void OnLowerValueChanged()
		{
			LowerValueChanged?.Invoke(this, EventArgs.Empty);
		}

		protected virtual void OnUpperValueChanged()
		{
			UpperValueChanged?.Invoke(this, EventArgs.Empty);
		}
	}



	public class PixelUtil
	{
		public static int SpToPx(Context context, int dp)
		{
			return (int)Math.Round(dp * context.Resources.DisplayMetrics.ScaledDensity);
		}

		public static int PxToSp(Context context, int px)
		{
			return (int)Math.Round(px / context.Resources.DisplayMetrics.ScaledDensity); ;
		}

		public static int DpToPx(Context context, int dp)
		{
			int px = (int)Math.Round(dp * GetPixelScaleFactor(context));
			return px;
		}

		public static int PxToDp(Context context, int px)
		{
			int dp = (int)Math.Round(px / GetPixelScaleFactor(context));
			return dp;
		}

		private static float GetPixelScaleFactor(Context context)
		{
			DisplayMetrics displayMetrics = context.Resources.DisplayMetrics;
			return displayMetrics.Xdpi / (int)DisplayMetricsDensity.Default;
		}
	}




	public class BitmapUtil
	{
		public static Bitmap DrawableToBitmap(Drawable drawable)
		{
			if (drawable is BitmapDrawable)
			{
				return ((BitmapDrawable)drawable).Bitmap;
			}

			// We ask for the bounds if they have been set as they would be most
			// correct, then we check we are  > 0
			var width = !drawable.Bounds.IsEmpty
				? drawable.Bounds.Width()
				: drawable.IntrinsicWidth;

			var height = !drawable.Bounds.IsEmpty
				? drawable.Bounds.Height()
				: drawable.IntrinsicHeight;

			// Now we check we are > 0
			var bitmap = Bitmap.CreateBitmap(width <= 0 ? 1 : width, height <= 0 ? 1 : height,
				Bitmap.Config.Argb8888);
			var canvas = new Canvas(bitmap);
			drawable.SetBounds(0, 0, canvas.Width, canvas.Height);
			drawable.Draw(canvas);

			return bitmap;
		}
	}
}
