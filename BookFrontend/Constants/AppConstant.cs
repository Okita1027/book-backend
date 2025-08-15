namespace book_frontend.Constants
{
    /// <summary>
    /// 应用程序常量定义
    /// 统一管理项目中的各种常量，提高代码可维护性
    /// </summary>
    [Obsolete("暂时不使用")]
    public static class AppConstant
    {
        /*
         * #region和#endregion必须成对出现
         * 仅用于IDE中的代码块折叠
         */
        #region UI 相关常量

        /// <summary>
        /// UI 颜色常量
        /// </summary>
        public static class Colors
        {
            // 主题色
            public const string Primary = "#2196F3";
            public const string PrimaryDark = "#1976D2";
            public const string PrimaryLight = "#7E57C2";
            public const string Accent = "#673AB7";
            public const string AccentDark = "#5E35B1";

            // 背景色
            public const string BackgroundLight = "#F8F9FA";
            public const string BackgroundGray = "#F5F5F5";
            public const string BackgroundWhite = "#FAFAFA";
            public const string BackgroundMask = "#80000000";
            public const string BackgroundSemiTransparent = "#80FFFFFF";

            // 边框色
            public const string BorderLight = "#E0E0E0";

            // 文字色
            public const string TextPrimary = "#333";
            public const string TextSecondary = "#555";
            public const string TextMuted = "#666";
            public const string TextGray = "Gray";
        }

        /// <summary>
        /// UI 尺寸常量
        /// </summary>
        public static class Sizes
        {
            // 字体大小
            public const double FontSizeSmall = 12.0;
            public const double FontSizeNormal = 14.0;
            public const double FontSizeMedium = 16.0;
            public const double FontSizeLarge = 20.0;
            public const double FontSizeXLarge = 28.0;

            // 间距
            public const double MarginSmall = 10.0;
            public const double MarginNormal = 16.0;
            public const double MarginLarge = 20.0;
            public const double MarginXLarge = 30.0;
            public const double MarginXXLarge = 40.0;

            // 内边距
            public const double PaddingSmall = 8.0;
            public const double PaddingNormal = 16.0;
            public const double PaddingLarge = 32.0;

            // 控件尺寸
            public const double ButtonHeight = 45.0;
            public const double IconSize = 24.0;
            public const double TextBoxWidth = 60.0;
            public const double ProgressBarWidth = 150.0;

            // 圆角
            public const double CornerRadiusSmall = 6.0;
            public const double CornerRadiusNormal = 10.0;

            // 阴影
            public const double ShadowDepthSmall = 2.0;
            public const double ShadowDepthNormal = 3.0;
            public const double ShadowOpacity = 0.3;
            public const double ShadowBlurRadius = 10.0;
            public const double ShadowDirection = 270.0;
        }

        #endregion

        #region 业务逻辑常量

        /// <summary>
        /// 分页相关常量
        /// </summary>
        public static class Pagination
        {
            public const int DefaultPageSize = 12;
            public const int MinPageSize = 1;
            public const int MaxPageSize = 100;
        }

        /// <summary>
        /// 网络请求相关常量
        /// </summary>
        public static class Network
        {
            public const int DefaultTimeoutSeconds = 30;
            public const int RetryCount = 3;
            public const int RetryDelayMilliseconds = 1000;
        }

        /// <summary>
        /// 滚动相关常量
        /// </summary>
        public static class Scroll
        {
            public const double LoadMoreThreshold = 100.0;
        }

        #endregion

        #region 配置相关常量

        /// <summary>
        /// 配置键名常量
        /// </summary>
        public static class ConfigKeys
        {
            public const string ApiBaseUrl = "ApiBaseUrl";
            public const string RequestTimeoutSeconds = "RequestTimeoutSeconds";
        }

        /// <summary>
        /// 默认配置值
        /// </summary>
        public static class DefaultConfig
        {
            public const string ApiBaseUrl = "http://localhost:8888/api/";
            public const int RequestTimeoutSeconds = 30;
        }

        #endregion

        #region 开发调试相关常量（不建议放在生产环境）

        /// <summary>
        /// 开发调试常量 - 仅用于开发环境
        /// 生产环境应该移除或通过配置文件管理
        /// </summary>
        public static class Debug
        {
            // 注意：这个密码常量仅用于开发调试，生产环境不应使用
            public const string DefaultTestPassword = "123456";
        }

        #endregion
    }
}