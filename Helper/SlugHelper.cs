using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace BookStore.Helper
{
    public class SlugHelper
    {
        public static string GenerateSlug(string name)
        {
            if (string.IsNullOrEmpty(name))
                return string.Empty;

            // Bước 1: Chuyển tiếng Việt có dấu về không dấu
            name = RemoveDiacritics(name);

            // Bước 2: ToLower, loại bỏ các ký tự đặc biệt (bao gồm cả dấu - gốc)
            string str = name.ToLower();
            str = Regex.Replace(str, @"[^a-z0-9\s]", ""); // chỉ giữ a-z, 0-9 và khoảng trắng

            // Bước 3: Rút gọn khoảng trắng và thay bằng dấu -
            str = Regex.Replace(str, @"\s+", "-"); // thay mọi khoảng trắng bằng dấu -

            // Bước 4: Rút gọn dấu - liên tiếp (phòng khi có)
            str = Regex.Replace(str, @"-+", "-"); // gộp nhiều dấu - liên tiếp thành 1

            // Bước 5: Xóa dấu - ở đầu/cuối (nếu có)
            str = str.Trim('-');

            return str;
        }

        // Hàm chuyển tiếng Việt có dấu thành không dấu
        private static string RemoveDiacritics(string text)
        {
            // Xử lý riêng ký tự Đ và đ
            text = text.Replace("Đ", "D").Replace("đ", "d");

            var normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in normalized)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
