using System.Collections.Generic;
using System.Globalization;

namespace LeafUpload.Web.Localization
{
    // Lightweight in-process translation lookup (English/Hindi/Urdu) - used from
    // Razor views as @T.S("key") and from JS-embedded strings via @Html.Raw(T.Json(...)).
    // Deliberately not resx/IStringLocalizer: this app's text lives in a handful of
    // files, so a single dictionary is easier to keep consistent than scattering
    // .resx files across every view.
    public static class T
    {
        public static string CurrentCulture => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName switch
        {
            "hi" => "hi",
            "ur" => "ur",
            _ => "en",
        };

        public static bool IsRtl => CurrentCulture == "ur";

        public static string S(string key)
        {
            var culture = CurrentCulture;
            if (Strings.TryGetValue(key, out var byCulture))
            {
                if (byCulture.TryGetValue(culture, out var value) && !string.IsNullOrEmpty(value))
                    return value;
                if (byCulture.TryGetValue("en", out var en))
                    return en;
            }
            return key;
        }

        // key -> culture -> text
        public static readonly Dictionary<string, Dictionary<string, string>> Strings = new()
        {
            // Navigation (shared header, every page)
            ["nav.home"] = new() { ["en"] = "Home", ["hi"] = "होम", ["ur"] = "ہوم" },
            ["nav.advisories"] = new() { ["en"] = "Crop Advisories", ["hi"] = "फ़सल सलाह", ["ur"] = "فصل کی مشاورت" },
            ["nav.diagnosis"] = new() { ["en"] = "Leaf Diagnosis", ["hi"] = "पत्ती निदान", ["ur"] = "پتی کی تشخیص" },
            ["nav.login"] = new() { ["en"] = "Login", ["hi"] = "लॉगिन", ["ur"] = "لاگ ان" },
            ["nav.register"] = new() { ["en"] = "Register", ["hi"] = "पंजीकरण", ["ur"] = "رجسٹریشن" },
            ["nav.logout"] = new() { ["en"] = "Logout", ["hi"] = "लॉगआउट", ["ur"] = "لاگ آؤٹ" },
            ["nav.loading"] = new() { ["en"] = "Loading…", ["hi"] = "लोड हो रहा है…", ["ur"] = "لوڈ ہو رہا ہے…" },
            ["nav.language"] = new() { ["en"] = "Language", ["hi"] = "भाषा", ["ur"] = "زبان" },
            ["footer.poweredBy"] = new() { ["en"] = "Powered by ML.NET", ["hi"] = "ML.NET द्वारा संचालित", ["ur"] = "ML.NET کے ذریعے تقویت یافتہ" },
            ["footer.credit"] = new() { ["en"] = "Built by Manager M&E & Manager IT", ["hi"] = "मैनेजर एम एंड ई और मैनेजर आईटी द्वारा निर्मित", ["ur"] = "منیجر ایم اینڈ ای اور منیجر آئی ٹی کی جانب سے تیار کردہ" },

            // Leaf Diagnosis page (Home/Index)
            ["diag.title"] = new() { ["en"] = "Leaf Disease Identification", ["hi"] = "पत्ती रोग पहचान", ["ur"] = "پتی کی بیماری کی شناخت" },
            ["diag.subtitle"] = new() { ["en"] = "Upload a photo and let the model diagnose it in seconds.", ["hi"] = "एक फ़ोटो अपलोड करें और मॉडल को सेकंडों में निदान करने दें।", ["ur"] = "ایک تصویر اپ لوڈ کریں اور ماڈل کو سیکنڈوں میں تشخیص کرنے دیں۔" },
            ["diag.dropText"] = new() { ["en"] = "Drop a leaf photo here, or click to browse", ["hi"] = "पत्ती की फ़ोटो यहाँ छोड़ें, या ब्राउज़ करने के लिए क्लिक करें", ["ur"] = "پتی کی تصویر یہاں ڈراپ کریں، یا براؤز کرنے کے لیے کلک کریں" },
            ["diag.dropHint"] = new() { ["en"] = "JPEG, PNG, WEBP or BMP · up to 10 MB", ["hi"] = "JPEG, PNG, WEBP या BMP · अधिकतम 10 MB", ["ur"] = "JPEG, PNG, WEBP یا BMP · زیادہ سے زیادہ 10 MB" },
            ["diag.button"] = new() { ["en"] = "Diagnose Leaf", ["hi"] = "पत्ती की जाँच करें", ["ur"] = "پتی کی جانچ کریں" },
            ["diag.analyzing"] = new() { ["en"] = "Analyzing your leaf…", ["hi"] = "आपकी पत्ती का विश्लेषण हो रहा है…", ["ur"] = "آپ کی پتی کا تجزیہ ہو رہا ہے…" },
            ["diag.healthy"] = new() { ["en"] = "Looking healthy", ["hi"] = "स्वस्थ दिख रही है", ["ur"] = "صحت مند نظر آ رہی ہے" },
            ["diag.result"] = new() { ["en"] = "Diagnosis result", ["hi"] = "निदान परिणाम", ["ur"] = "تشخیص کا نتیجہ" },
            ["diag.confidence"] = new() { ["en"] = "Confidence", ["hi"] = "विश्वास स्तर", ["ur"] = "اعتماد کی سطح" },
            ["diag.symptomsTitle"] = new() { ["en"] = "Signs & symptoms to verify", ["hi"] = "पुष्टि हेतु लक्षण जाँचें", ["ur"] = "تصدیق کے لیے علامات جانچیں" },
            ["diag.symptomsHint"] = new() { ["en"] = "A photo is one data point, not a lab test - check your plant for these signs before treating.", ["hi"] = "एक फ़ोटो केवल एक संकेत है, प्रयोगशाला जाँच नहीं - उपचार से पहले अपने पौधे में ये लक्षण जाँच लें।", ["ur"] = "ایک تصویر صرف ایک اشارہ ہے، لیبارٹری ٹیسٹ نہیں - علاج سے پہلے اپنے پودے میں یہ علامات ضرور جانچیں۔" },
            ["diag.recommendedAction"] = new() { ["en"] = "Recommended action", ["hi"] = "अनुशंसित कार्रवाई", ["ur"] = "تجویز کردہ اقدام" },
            ["diag.kkgPrompt"] = new() { ["en"] = "Want to talk to a real expert about this?", ["hi"] = "क्या आप इस बारे में किसी विशेषज्ञ से बात करना चाहते हैं?", ["ur"] = "کیا آپ اس بارے میں کسی ماہر سے بات کرنا چاہتے ہیں؟" },
            ["diag.kkgButton"] = new() { ["en"] = "📱 Chat / Video Call an Expert on KKG App", ["hi"] = "📱 KKG ऐप पर विशेषज्ञ से चैट/वीडियो कॉल करें", ["ur"] = "📱 KKG ایپ پر ماہر سے چیٹ/ویڈیو کال کریں" },
            ["diag.errorInvalidImage"] = new() { ["en"] = "Please upload a valid image.", ["hi"] = "कृपया एक मान्य छवि अपलोड करें।", ["ur"] = "براہ کرم ایک درست تصویر اپ لوڈ کریں۔" },
            ["diag.errorTooLarge"] = new() { ["en"] = "File is too large. Maximum size is 10 MB.", ["hi"] = "फ़ाइल बहुत बड़ी है। अधिकतम आकार 10 MB है।", ["ur"] = "فائل بہت بڑی ہے۔ زیادہ سے زیادہ سائز 10 MB ہے۔" },
            ["diag.errorUnsupportedType"] = new() { ["en"] = "Unsupported file type. Please upload a JPEG, PNG, WEBP, or BMP image.", ["hi"] = "असमर्थित फ़ाइल प्रकार। कृपया JPEG, PNG, WEBP, या BMP छवि अपलोड करें।", ["ur"] = "غیر معاون فائل قسم۔ براہ کرم JPEG, PNG, WEBP یا BMP تصویر اپ لوڈ کریں۔" },
            ["diag.tipsTitle"] = new() { ["en"] = "Tips for a Clear Photo", ["hi"] = "स्पष्ट फ़ोटो के लिए सुझाव", ["ur"] = "واضح تصویر کے لیے تجاویز" },
            ["diag.tip1"] = new() { ["en"] = "Photograph a single leaf in bright, natural daylight.", ["hi"] = "एक ही पत्ती की तेज़, प्राकृतिक धूप में फ़ोटो लें।", ["ur"] = "ایک ہی پتی کی تیز، قدرتی روشنی میں تصویر لیں۔" },
            ["diag.tip2"] = new() { ["en"] = "Fill the frame with the leaf - avoid heavy shadows or blur.", ["hi"] = "पत्ती को पूरे फ्रेम में भरें - गहरी छाया या धुंधलापन से बचें।", ["ur"] = "پتی کو پورے فریم میں بھریں - گہرے سائے یا دھندلاپن سے بچیں۔" },
            ["diag.tip3"] = new() { ["en"] = "Capture the affected area closely: spots, discoloration, or curling.", ["hi"] = "प्रभावित हिस्से को नज़दीक से कैद करें: धब्बे, रंग बदलना, या मुड़ना।", ["ur"] = "متاثرہ حصے کو قریب سے کیپچر کریں: دھبے، رنگت میں تبدیلی، یا مڑنا۔" },
            ["diag.tip4"] = new() { ["en"] = "One clear photo works better than several blurry ones.", ["hi"] = "एक स्पष्ट फ़ोटो कई धुंधली फ़ोटो से बेहतर काम करती है।", ["ur"] = "ایک واضح تصویر کئی دھندلی تصاویر سے بہتر کام کرتی ہے۔" },
            ["diag.howTitle"] = new() { ["en"] = "How Diagnosis Works", ["hi"] = "निदान कैसे काम करता है", ["ur"] = "تشخیص کیسے کام کرتی ہے" },
            ["diag.howBody"] = new() { ["en"] = "Your photo is analyzed by an on-device ML model trained on thousands of leaf images across 14 crops. You'll get a likely disease, a confidence score, and symptoms to double-check before treating.", ["hi"] = "आपकी फ़ोटो का विश्लेषण एक ऑन-डिवाइस ML मॉडल द्वारा किया जाता है जिसे 14 फ़सलों की हज़ारों पत्ती छवियों पर प्रशिक्षित किया गया है। आपको संभावित रोग, विश्वास स्कोर, और उपचार से पहले जाँचने योग्य लक्षण मिलेंगे।", ["ur"] = "آپ کی تصویر کا تجزیہ ایک آن ڈیوائس ML ماڈل کے ذریعے کیا جاتا ہے جسے 14 فصلوں کی ہزاروں پتی تصاویر پر تربیت دی گئی ہے۔ آپ کو ممکنہ بیماری، اعتماد کا اسکور، اور علاج سے پہلے جانچنے کے لیے علامات ملیں گی۔" },

            // Shared form field labels (Login + Register)
            ["field.username"] = new() { ["en"] = "Username", ["hi"] = "उपयोगकर्ता नाम", ["ur"] = "صارف نام" },
            ["field.password"] = new() { ["en"] = "Password", ["hi"] = "पासवर्ड", ["ur"] = "پاس ورڈ" },
            ["field.confirmPassword"] = new() { ["en"] = "Confirm Password", ["hi"] = "पासवर्ड की पुष्टि करें", ["ur"] = "پاس ورڈ کی تصدیق کریں" },
            ["field.location"] = new() { ["en"] = "Farm location (village/town)", ["hi"] = "खेत का स्थान (गाँव/शहर)", ["ur"] = "کھیت کا مقام (گاؤں/شہر)" },
            ["field.cropType"] = new() { ["en"] = "Crop type", ["hi"] = "फ़सल का प्रकार", ["ur"] = "فصل کی قسم" },

            // Login page
            ["login.title"] = new() { ["en"] = "Welcome Back", ["hi"] = "वापसी पर स्वागत है", ["ur"] = "خوش آمدید" },
            ["login.subtitle"] = new() { ["en"] = "Log in to view your farm's crop advisories.", ["hi"] = "अपने खेत की फ़सल सलाह देखने के लिए लॉगिन करें।", ["ur"] = "اپنے کھیت کی فصل کی مشاورت دیکھنے کے لیے لاگ ان کریں۔" },
            ["login.button"] = new() { ["en"] = "Log In", ["hi"] = "लॉगिन करें", ["ur"] = "لاگ ان کریں" },
            ["login.registerLink"] = new() { ["en"] = "New here? Register your farm", ["hi"] = "यहाँ नए हैं? अपना खेत पंजीकृत करें", ["ur"] = "یہاں نئے ہیں؟ اپنا کھیت رجسٹر کریں" },
            ["login.sideTitle"] = new() { ["en"] = "Why Farmers Use KrishiMitra AI", ["hi"] = "किसान KrishiMitra AI का उपयोग क्यों करते हैं", ["ur"] = "کسان KrishiMitra AI کیوں استعمال کرتے ہیں" },
            ["login.sideTip1"] = new() { ["en"] = "Instant leaf disease diagnosis from a photo", ["hi"] = "फ़ोटो से तुरंत पत्ती रोग निदान", ["ur"] = "تصویر سے فوری پتی کی بیماری کی تشخیص" },
            ["login.sideTip2"] = new() { ["en"] = "Weather-driven crop advisories for your farm", ["hi"] = "आपके खेत के लिए मौसम आधारित फ़सल सलाह", ["ur"] = "آپ کے کھیت کے لیے موسم پر مبنی فصل کی مشاورت" },
            ["login.sideTip3"] = new() { ["en"] = "Chat or video call a real expert via the KKG app", ["hi"] = "KKG ऐप के ज़रिए किसी विशेषज्ञ से चैट या वीडियो कॉल करें", ["ur"] = "KKG ایپ کے ذریعے کسی ماہر سے چیٹ یا ویڈیو کال کریں" },

            // Register page
            ["register.title"] = new() { ["en"] = "Register Your Farm", ["hi"] = "अपना खेत पंजीकृत करें", ["ur"] = "اپنا کھیت رجسٹر کریں" },
            ["register.subtitle"] = new() { ["en"] = "Create an account and tell us about your farm to start receiving crop advisories.", ["hi"] = "फ़सल सलाह प्राप्त करने के लिए खाता बनाएं और अपने खेत के बारे में बताएं।", ["ur"] = "فصل کی مشاورت حاصل کرنے کے لیے اکاؤنٹ بنائیں اور اپنے کھیت کے بارے میں بتائیں۔" },
            ["register.placeholderLocation"] = new() { ["en"] = "e.g. Anantnag", ["hi"] = "उदाहरण: अनंतनाग", ["ur"] = "مثلاً اننت ناگ" },
            ["register.locate"] = new() { ["en"] = "📍 Locate", ["hi"] = "📍 खोजें", ["ur"] = "📍 تلاش کریں" },
            ["register.useMyLocation"] = new() { ["en"] = "🧭 Use my current location", ["hi"] = "🧭 मेरा वर्तमान स्थान उपयोग करें", ["ur"] = "🧭 میری موجودہ لوکیشن استعمال کریں" },
            ["register.mapHint"] = new() { ["en"] = "Click or drag the pin to your exact field location.", ["hi"] = "पिन को अपने खेत के सटीक स्थान पर क्लिक या खींचें।", ["ur"] = "پن کو اپنے کھیت کی صحیح جگہ پر کلک یا گھسیٹیں۔" },
            ["register.selectCrop"] = new() { ["en"] = "Select a crop…", ["hi"] = "फ़सल चुनें…", ["ur"] = "فصل منتخب کریں…" },
            ["register.button"] = new() { ["en"] = "Register Farm", ["hi"] = "खेत पंजीकृत करें", ["ur"] = "کھیت رجسٹر کریں" },
            ["register.loginLink"] = new() { ["en"] = "Already have an account? Log in", ["hi"] = "पहले से खाता है? लॉगिन करें", ["ur"] = "پہلے سے اکاؤنٹ ہے؟ لاگ ان کریں" },
            ["register.typeLocationFirst"] = new() { ["en"] = "Type a location first.", ["hi"] = "पहले एक स्थान लिखें।", ["ur"] = "پہلے ایک مقام لکھیں۔" },
            ["register.searching"] = new() { ["en"] = "Searching…", ["hi"] = "खोजा जा रहा है…", ["ur"] = "تلاش ہو رہی ہے…" },
            ["register.notFoundManualPin"] = new() { ["en"] = "Couldn't find that name automatically - click or drag the pin below to your farm's location.", ["hi"] = "वह नाम स्वतः नहीं मिला - नीचे पिन को अपने खेत के स्थान पर क्लिक या खींचें।", ["ur"] = "وہ نام خودکار طریقے سے نہیں ملا - نیچے پن کو اپنے کھیت کی جگہ پر کلک یا گھسیٹیں۔" },
            ["register.adjustPin"] = new() { ["en"] = "drag the pin if it needs adjusting.", ["hi"] = "ज़रूरत हो तो पिन को समायोजित करने के लिए खींचें।", ["ur"] = "ضرورت ہو تو پن کو ایڈجسٹ کرنے کے لیے گھسیٹیں۔" },
            ["register.searchError"] = new() { ["en"] = "Something went wrong looking that up. Try again.", ["hi"] = "खोजते समय कुछ गड़बड़ हुई। पुनः प्रयास करें।", ["ur"] = "تلاش کرتے وقت کچھ غلط ہوا۔ دوبارہ کوشش کریں۔" },
            ["register.geoUnsupported"] = new() { ["en"] = "Your browser doesn't support location access - use Locate or the map instead.", ["hi"] = "आपका ब्राउज़र स्थान एक्सेस समर्थित नहीं करता - इसके बजाय खोजें या मानचित्र का उपयोग करें।", ["ur"] = "آپ کا براؤزر لوکیشن تک رسائی کو سپورٹ نہیں کرتا - اس کے بجائے تلاش کریں یا نقشہ استعمال کریں۔" },
            ["register.requestingLocation"] = new() { ["en"] = "Requesting location access…", ["hi"] = "स्थान एक्सेस का अनुरोध किया जा रहा है…", ["ur"] = "لوکیشن تک رسائی کی درخواست کی جا رہی ہے…" },
            ["register.locatedLookingUp"] = new() { ["en"] = "Located - looking up the place name…", ["hi"] = "स्थान मिला - जगह का नाम खोजा जा रहा है…", ["ur"] = "لوکیشن مل گئی - جگہ کا نام تلاش کیا جا رہا ہے…" },
            ["register.locatedManualName"] = new() { ["en"] = "Located you on the map - drag the pin to fine-tune, and type a place name above.", ["hi"] = "आपको मानचित्र पर स्थित किया - सटीक करने के लिए पिन खींचें और ऊपर जगह का नाम लिखें।", ["ur"] = "آپ کو نقشے پر لوکیٹ کیا - درستگی کے لیے پن گھسیٹیں اور اوپر جگہ کا نام لکھیں۔" },
            ["register.geoDenied"] = new() { ["en"] = "Location access was denied - use Locate or click the map instead.", ["hi"] = "स्थान एक्सेस अस्वीकृत हुआ - इसके बजाय खोजें या मानचित्र पर क्लिक करें।", ["ur"] = "لوکیشن تک رسائی مسترد کر دی گئی - اس کے بجائے تلاش کریں یا نقشے پر کلک کریں۔" },
            ["register.geoFailed"] = new() { ["en"] = "Couldn't get your location - use Locate or click the map instead.", ["hi"] = "आपका स्थान प्राप्त नहीं हो सका - इसके बजाय खोजें या मानचित्र पर क्लिक करें।", ["ur"] = "آپ کی لوکیشن حاصل نہیں ہو سکی - اس کے بجائے تلاش کریں یا نقشے پر کلک کریں۔" },
            ["register.sideTitle"] = new() { ["en"] = "What You Get After Registering", ["hi"] = "पंजीकरण के बाद आपको क्या मिलेगा", ["ur"] = "رجسٹریشن کے بعد آپ کو کیا ملے گا" },
            ["register.sideTip1"] = new() { ["en"] = "A 7-day weather forecast tailored to your farm's exact location", ["hi"] = "आपके खेत के सटीक स्थान के अनुसार 7-दिन का मौसम पूर्वानुमान", ["ur"] = "آپ کے کھیت کی صحیح جگہ کے مطابق 7 دن کی موسمی پیش گوئی" },
            ["register.sideTip2"] = new() { ["en"] = "Automatic alerts for hail, frost, heat waves, and more", ["hi"] = "ओलावृष्टि, पाला, लू आदि के लिए स्वचालित चेतावनियाँ", ["ur"] = "اولے، پالا، شدید گرمی وغیرہ کے لیے خودکار انتباہات" },
            ["register.sideTip3"] = new() { ["en"] = "One-tap access to expert help through the KKG app", ["hi"] = "KKG ऐप के ज़रिए विशेषज्ञ सहायता तक एक-टैप पहुँच", ["ur"] = "KKG ایپ کے ذریعے ماہر کی مدد تک ایک ٹیپ رسائی" },

            // Crop Advisories page
            ["advisory.title"] = new() { ["en"] = "Crop Advisories", ["hi"] = "फ़सल सलाह", ["ur"] = "فصل کی مشاورت" },
            ["advisory.subtitleOne"] = new() { ["en"] = "Weather-driven guidance for your registered farm.", ["hi"] = "आपके पंजीकृत खेत के लिए मौसम आधारित मार्गदर्शन।", ["ur"] = "آپ کے رجسٹرڈ کھیت کے لیے موسم پر مبنی رہنمائی۔" },
            ["advisory.subtitleMany"] = new() { ["en"] = "Weather-driven guidance for your registered farms.", ["hi"] = "आपके पंजीकृत खेतों के लिए मौसम आधारित मार्गदर्शन।", ["ur"] = "آپ کے رجسٹرڈ کھیتوں کے لیے موسم پر مبنی رہنمائی۔" },
            ["advisory.noFarm"] = new() { ["en"] = "You don't have a registered farm yet.", ["hi"] = "आपके पास अभी कोई पंजीकृत खेत नहीं है।", ["ur"] = "آپ کے پاس ابھی کوئی رجسٹرڈ کھیت نہیں ہے۔" },
            ["advisory.registerOne"] = new() { ["en"] = "Register one", ["hi"] = "एक पंजीकृत करें", ["ur"] = "ایک رجسٹر کریں" },
            ["advisory.toStartGetting"] = new() { ["en"] = "to start getting advisories.", ["hi"] = "सलाह प्राप्त करना शुरू करने के लिए।", ["ur"] = "مشاورت حاصل کرنا شروع کرنے کے لیے۔" },
            ["advisory.allClear"] = new() { ["en"] = "All clear", ["hi"] = "सब ठीक है", ["ur"] = "سب ٹھیک ہے" },
            ["advisory.alert"] = new() { ["en"] = "alert", ["hi"] = "चेतावनी", ["ur"] = "انتباہ" },
            ["advisory.alerts"] = new() { ["en"] = "alerts", ["hi"] = "चेतावनियाँ", ["ur"] = "انتباہات" },
            ["advisory.pendingTitle"] = new() { ["en"] = "Advisory pending", ["hi"] = "सलाह प्रतीक्षित है", ["ur"] = "مشاورت زیر التوا ہے" },
            ["advisory.pendingMessage"] = new() { ["en"] = "No advisory available yet for this farm - check back shortly.", ["hi"] = "इस खेत के लिए अभी कोई सलाह उपलब्ध नहीं है - जल्द ही फिर देखें।", ["ur"] = "اس کھیت کے لیے ابھی کوئی مشاورت دستیاب نہیں ہے - جلد دوبارہ چیک کریں۔" },
            ["advisory.generated"] = new() { ["en"] = "Generated", ["hi"] = "तैयार किया गया", ["ur"] = "تیار کیا گیا" },
            ["advisory.aiGenerated"] = new() { ["en"] = "AI-generated", ["hi"] = "AI द्वारा तैयार", ["ur"] = "AI کے ذریعے تیار کردہ" },
            ["advisory.ruleBased"] = new() { ["en"] = "Rule-based engine", ["hi"] = "नियम-आधारित इंजन", ["ur"] = "قاعدہ پر مبنی انجن" },
            ["advisory.pushToFarmer"] = new() { ["en"] = "📲 Push to Farmer", ["hi"] = "📲 किसान को भेजें", ["ur"] = "📲 کسان کو بھیجیں" },
            ["advisory.sharePrompt"] = new() { ["en"] = "Know a farmer who'd benefit from weather-smart advisories? Share KrishiMitra AI.", ["hi"] = "किसी ऐसे किसान को जानते हैं जिन्हें मौसम-आधारित सलाह से फ़ायदा होगा? KrishiMitra AI साझा करें।", ["ur"] = "کیا آپ کسی ایسے کسان کو جانتے ہیں جسے موسمی مشاورت سے فائدہ ہو؟ KrishiMitra AI شیئر کریں۔" },
            ["advisory.shareWhatsapp"] = new() { ["en"] = "WhatsApp", ["hi"] = "व्हाट्सऐप", ["ur"] = "واٹس ایپ" },
            ["advisory.shareFacebook"] = new() { ["en"] = "Facebook", ["hi"] = "फ़ेसबुक", ["ur"] = "فیس بک" },
            ["advisory.shareTwitter"] = new() { ["en"] = "X / Twitter", ["hi"] = "X / ट्विटर", ["ur"] = "X / ٹویٹر" },
            ["advisory.shareCopy"] = new() { ["en"] = "Copy Link", ["hi"] = "लिंक कॉपी करें", ["ur"] = "لنک کاپی کریں" },
            ["advisory.shareCopied"] = new() { ["en"] = "Link copied!", ["hi"] = "लिंक कॉपी हो गया!", ["ur"] = "لنک کاپی ہو گیا!" },

            // Landing page
            ["landing.heroTitle"] = new() { ["en"] = "Weather-Smart Farming for Jammu & Kashmir", ["hi"] = "जम्मू और कश्मीर के लिए मौसम-स्मार्ट खेती", ["ur"] = "جموں و کشمیر کے لیے موسم کے مطابق ذہین کاشتکاری" },
            ["landing.heroSubtitle"] = new() { ["en"] = "Krishi Mitra AI is JKCIP's intelligent crop advisory platform that combines weather intelligence and Artificial Intelligence to help farmers make better decisions. Get farm-specific weather advisories, early risk alerts, and diagnose plant diseases instantly using our AI-powered Leaf Diagnosis tool—all in one place.", ["hi"] = "KrishiMitra AI जम्मू और कश्मीर भर के किसानों को पत्ती रोग की तुरंत पहचान करने और मौसम आधारित फ़सल सलाह प्राप्त करने में मदद करता है - हर पंजीकृत खेत के लिए निःशुल्क। यह जम्मू और कश्मीर कृषि एवं संबद्ध क्षेत्र प्रतिस्पर्धात्मकता सुधार परियोजना (JKCIP) के समर्थन में बागवानी एवं कृषि विकास कार्यक्रम (HADP) के अंतर्गत बनाया गया है।", ["ur"] = "KrishiMitra AI جموں و کشمیر بھر کے کسانوں کو پتی کی بیماری کی فوری تشخیص اور موسم پر مبنی فصل کی مشاورت حاصل کرنے میں مدد دیتا ہے - ہر رجسٹرڈ کھیت کے لیے مفت۔ یہ جموں و کشمیر زراعت اور متعلقہ شعبہ جات مسابقتی بہتری منصوبہ (JKCIP) کی معاونت میں باغبانی و زراعت ترقیاتی پروگرام (HADP) کے تحت تیار کیا گیا ہے۔" },
            ["landing.ctaRegister"] = new() { ["en"] = "🌾 Register Your Farm", ["hi"] = "🌾 अपना खेत पंजीकृत करें", ["ur"] = "🌾 اپنا کھیت رجسٹر کریں" },
            ["landing.ctaAdvisories"] = new() { ["en"] = "🌦️ View Crop Advisories", ["hi"] = "🌦️ फ़सल सलाह देखें", ["ur"] = "🌦️ فصل کی مشاورت دیکھیں" },
            ["landing.ctaDiagnosis"] = new() { ["en"] = "🔍 Try Leaf Diagnosis", ["hi"] = "🔍 पत्ती निदान आज़माएँ", ["ur"] = "🔍 پتی کی تشخیص آزمائیں" },
            ["landing.statFarmers"] = new() { ["en"] = "👤 Farmers Onboarded", ["hi"] = "👤 पंजीकृत किसान", ["ur"] = "👤 رجسٹرڈ کسان" },
            ["landing.statFarms"] = new() { ["en"] = "🌱 Farms Registered", ["hi"] = "🌱 प्लेटफ़ॉर्म पर खेत", ["ur"] = "🌱 پلیٹ فارم پر کھیت" },
            ["landing.statCrops"] = new() { ["en"] = "🏭 Units Tracking", ["hi"] = "🍎 समर्थित फ़सल किस्में", ["ur"] = "🍎 معاون فصل کی اقسام" },
            ["landing.statAlerts"] = new() { ["en"] = "🏢 Kisan Khidmat Ghars Onboarded", ["hi"] = "⚠️ सक्रिय मौसम चेतावनियाँ", ["ur"] = "⚠️ فعال موسمی انتباہات" },
            ["landing.aboutTitle"] = new() { ["en"] = "About the JKCIP Programme", ["hi"] = "JKCIP कार्यक्रम के बारे में", ["ur"] = "JKCIP پروگرام کے بارے میں" },
            ["landing.aboutBody"] = new() { ["en"] = "This advisory tool supports the Jammu & Kashmir Competitiveness Improvement of Agriculture and Allied Sectors Project (JKCIP), an IFAD-supported initiative under the Horticulture & Agriculture Development Programme (HADP). JKCIP works with farmers across the Union Territory to improve the productivity, climate resilience, and market access of high-value agriculture, horticulture, and allied-sector crops through a value-chain approach - from production, to value addition, to market linkages.", ["hi"] = "यह सलाह उपकरण जम्मू और कश्मीर कृषि एवं संबद्ध क्षेत्र प्रतिस्पर्धात्मकता सुधार परियोजना (JKCIP) का समर्थन करता है, जो बागवानी एवं कृषि विकास कार्यक्रम (HADP) के अंतर्गत IFAD-समर्थित पहल है। JKCIP केंद्र शासित प्रदेश भर के किसानों के साथ मिलकर उत्पादकता, जलवायु सहनशीलता और उच्च-मूल्य कृषि, बागवानी व संबद्ध क्षेत्र की फ़सलों की बाज़ार पहुँच को उत्पादन से लेकर मूल्यवर्धन और बाज़ार संपर्क तक की मूल्य-श्रृंखला के माध्यम से बेहतर बनाता है।", ["ur"] = "یہ مشاورتی ٹول جموں و کشمیر زراعت اور متعلقہ شعبہ جات مسابقتی بہتری منصوبہ (JKCIP) کی معاونت کرتا ہے، جو باغبانی و زراعت ترقیاتی پروگرام (HADP) کے تحت IFAD کی معاونت یافتہ پہل ہے۔ JKCIP یونین ٹیریٹری بھر کے کسانوں کے ساتھ مل کر پیداواریت، موسمیاتی لچک اور اعلیٰ قدر کی زراعت، باغبانی اور متعلقہ شعبہ جات کی فصلوں کی مارکیٹ تک رسائی کو پیداوار سے لے کر قدر میں اضافہ اور مارکیٹ روابط تک کے ویلیو چین کے ذریعے بہتر بناتا ہے۔" },
            ["landing.aboutReadMore"] = new() { ["en"] = "Read more at the official JKCIP portal.", ["hi"] = "अधिक जानकारी के लिए आधिकारिक JKCIP पोर्टल देखें।", ["ur"] = "مزید معلومات کے لیے سرکاری JKCIP پورٹل ملاحظہ کریں۔" },
            ["landing.aboutReadMoreLink"] = new() { ["en"] = "official JKCIP portal", ["hi"] = "आधिकारिक JKCIP पोर्टल", ["ur"] = "سرکاری JKCIP پورٹل" },
            ["landing.statWomen"] = new() { ["en"] = "Women-led households reached", ["hi"] = "महिला-नेतृत्व वाले घर तक पहुँच", ["ur"] = "خواتین کی سربراہی والے گھرانوں تک رسائی" },
            ["landing.statYouth"] = new() { ["en"] = "Youth households reached", ["hi"] = "युवा घरों तक पहुँच", ["ur"] = "نوجوانوں کے گھرانوں تک رسائی" },
            ["landing.statVulnerable"] = new() { ["en"] = "Vulnerable-community households", ["hi"] = "कमज़ोर समुदाय के घर", ["ur"] = "کمزور طبقے کے گھرانے" },
            ["landing.statSource"] = new() { ["en"] = "Programme-wide figures published on the official JKCIP portal, shown here for context - not specific to this tool.", ["hi"] = "आधिकारिक JKCIP पोर्टल पर प्रकाशित कार्यक्रम-व्यापी आंकड़े, यहाँ केवल संदर्भ हेतु दिखाए गए हैं - यह इस उपकरण के विशिष्ट आंकड़े नहीं हैं।", ["ur"] = "سرکاری JKCIP پورٹل پر شائع شدہ پروگرام گیر اعداد و شمار، یہاں صرف حوالے کے لیے دکھائے گئے ہیں - یہ اس ٹول کے مخصوص اعداد و شمار نہیں ہیں۔" },
            ["landing.focus1Title"] = new() { ["en"] = "Climate-Smart, Market-Led Production", ["hi"] = "जलवायु-अनुकूल, बाज़ार-केंद्रित उत्पादन", ["ur"] = "موسمیاتی طور پر ذہین، مارکیٹ پر مبنی پیداوار" },
            ["landing.focus1Body"] = new() { ["en"] = "Helping farmers adopt climate-resilient practices and diversify into high-value niche and horticultural crops.", ["hi"] = "किसानों को जलवायु-सहनशील प्रथाएँ अपनाने और उच्च-मूल्य विशिष्ट व बागवानी फ़सलों में विविधता लाने में मदद करना।", ["ur"] = "کسانوں کو موسمیاتی لچکدار طریقے اپنانے اور اعلیٰ قدر کی خصوصی و باغبانی فصلوں میں تنوع لانے میں مدد دینا۔" },
            ["landing.focus2Title"] = new() { ["en"] = "Agri-Business Ecosystem Development", ["hi"] = "कृषि-व्यवसाय पारिस्थितिकी तंत्र विकास", ["ur"] = "زرعی کاروباری ماحولیاتی نظام کی ترقی" },
            ["landing.focus2Body"] = new() { ["en"] = "Strengthening farmer collectives and the value chain that connects fields to processors and markets.", ["hi"] = "किसान समूहों और खेतों को प्रोसेसर व बाज़ारों से जोड़ने वाली मूल्य-श्रृंखला को मज़बूत बनाना।", ["ur"] = "کسان تنظیموں اور کھیتوں کو پروسیسرز اور مارکیٹوں سے جوڑنے والی ویلیو چین کو مضبوط بنانا۔" },
            ["landing.focus3Title"] = new() { ["en"] = "Support for Vulnerable Communities", ["hi"] = "कमज़ोर समुदायों के लिए सहायता", ["ur"] = "کمزور طبقات کے لیے معاونت" },
            ["landing.focus3Body"] = new() { ["en"] = "Targeted support for women, youth, and vulnerable households to share in agricultural growth.", ["hi"] = "महिलाओं, युवाओं और कमज़ोर परिवारों को कृषि विकास में भागीदार बनाने हेतु लक्षित सहायता।", ["ur"] = "خواتین، نوجوانوں اور کمزور گھرانوں کو زرعی ترقی میں شریک بنانے کے لیے مخصوص معاونت۔" },
            ["landing.insightsTitle"] = new() { ["en"] = "📊 Platform Insights", ["hi"] = "📊 प्लेटफ़ॉर्म अंतर्दृष्टि", ["ur"] = "📊 پلیٹ فارم بصیرت" },
            ["landing.insightsSubtitle"] = new() { ["en"] = "Live data from farms actually registered on this portal - grows as more farmers join.", ["hi"] = "इस पोर्टल पर वास्तव में पंजीकृत खेतों का लाइव डेटा - अधिक किसानों के जुड़ने के साथ बढ़ता है।", ["ur"] = "اس پورٹل پر واقعی رجسٹرڈ کھیتوں کا لائیو ڈیٹا - مزید کسانوں کے شامل ہونے کے ساتھ بڑھتا ہے۔" },
            ["landing.cropChartTitle"] = new() { ["en"] = "Registered Crops Breakdown", ["hi"] = "पंजीकृत फ़सलों का विवरण", ["ur"] = "رجسٹرڈ فصلوں کی تفصیل" },
            ["landing.cropChartEmpty"] = new() { ["en"] = "No farms registered yet - be the first! 🌱", ["hi"] = "अभी तक कोई खेत पंजीकृत नहीं - पहले बनें! 🌱", ["ur"] = "ابھی تک کوئی کھیت رجسٹرڈ نہیں - پہلے بنیں! 🌱" },
            ["landing.alertChartTitle"] = new() { ["en"] = "Active Weather Risk Alerts", ["hi"] = "सक्रिय मौसम जोखिम चेतावनियाँ", ["ur"] = "فعال موسمی خطرے کے انتباہات" },
            ["landing.alertChartEmpty"] = new() { ["en"] = "No active weather risks right now. ✅", ["hi"] = "फ़िलहाल कोई सक्रिय मौसम जोखिम नहीं है। ✅", ["ur"] = "فی الحال کوئی فعال موسمی خطرہ نہیں ہے۔ ✅" },
            ["landing.howItWorksTitle"] = new() { ["en"] = "⚙️ How It Works", ["hi"] = "⚙️ यह कैसे काम करता है", ["ur"] = "⚙️ یہ کیسے کام کرتا ہے" },
            ["landing.step1Title"] = new() { ["en"] = "Register & Locate", ["hi"] = "पंजीकरण करें और स्थान बताएं", ["ur"] = "رجسٹر کریں اور مقام بتائیں" },
            ["landing.step1Body"] = new() { ["en"] = "Create a free account, tell us your crop, and pin your farm's exact location on the map.", ["hi"] = "मुफ़्त खाता बनाएं, अपनी फ़सल बताएं, और मानचित्र पर अपने खेत का सटीक स्थान अंकित करें।", ["ur"] = "مفت اکاؤنٹ بنائیں، اپنی فصل بتائیں، اور نقشے پر اپنے کھیت کا صحیح مقام نشان زد کریں۔" },
            ["landing.step2Title"] = new() { ["en"] = "We Watch the Weather", ["hi"] = "हम मौसम पर नज़र रखते हैं", ["ur"] = "ہم موسم پر نظر رکھتے ہیں" },
            ["landing.step2Body"] = new() { ["en"] = "Every day we pull a 7-day forecast for your farm's coordinates from Open-Meteo.", ["hi"] = "हर दिन हम Open-Meteo से आपके खेत के निर्देशांक हेतु 7-दिन का पूर्वानुमान प्राप्त करते हैं।", ["ur"] = "ہر روز ہم Open-Meteo سے آپ کے کھیت کی جگہ کے لیے 7 دن کی پیش گوئی حاصل کرتے ہیں۔" },
            ["landing.step3Title"] = new() { ["en"] = "Get Actionable Advisories", ["hi"] = "व्यावहारिक सलाह प्राप्त करें", ["ur"] = "قابل عمل مشاورت حاصل کریں" },
            ["landing.step3Body"] = new() { ["en"] = "Our engine flags hail, frost, heat waves and more, with crop-specific guidance on what to do next.", ["hi"] = "हमारा इंजन ओलावृष्टि, पाला, लू आदि की पहचान करता है और फ़सल-विशिष्ट मार्गदर्शन देता है कि आगे क्या करें।", ["ur"] = "ہمارا انجن اولے، پالا، شدید گرمی وغیرہ کی نشاندہی کرتا ہے اور فصل کے مطابق رہنمائی دیتا ہے کہ آگے کیا کرنا ہے۔" },
            ["landing.cropsTitle"] = new() { ["en"] = "🌾 Crops We Support", ["hi"] = "🌾 समर्थित फ़सलें", ["ur"] = "🌾 معاون فصلیں" },
            ["landing.cropsSubtitle"] = new() { ["en"] = "The leaf-disease model and crop advisories both cover these 14 crops.", ["hi"] = "पत्ती-रोग मॉडल और फ़सल सलाह दोनों इन 14 फ़सलों को कवर करते हैं।", ["ur"] = "پتی کی بیماری کا ماڈل اور فصل کی مشاورت دونوں ان 14 فصلوں کا احاطہ کرتے ہیں۔" },
            ["landing.risksTitle"] = new() { ["en"] = "⚠️ Weather Risks We Monitor", ["hi"] = "⚠️ हम जिन मौसम जोखिमों की निगरानी करते हैं", ["ur"] = "⚠️ ہم جن موسمی خطرات کی نگرانی کرتے ہیں" },
            ["landing.risksSubtitle"] = new() { ["en"] = "Our advisory engine watches for each of these in your farm's 7-day forecast.", ["hi"] = "हमारा सलाह इंजन आपके खेत के 7-दिन के पूर्वानुमान में इन सभी पर नज़र रखता है।", ["ur"] = "ہمارا مشاورتی انجن آپ کے کھیت کی 7 دن کی پیش گوئی میں ان سب پر نظر رکھتا ہے۔" },
            ["landing.riskHailTitle"] = new() { ["en"] = "Hail", ["hi"] = "ओलावृष्टि", ["ur"] = "اولے" },
            ["landing.riskHailBody"] = new() { ["en"] = "Thunderstorms with hail can shred leaves and bruise or split fruit within minutes, especially damaging for apples, grapes, and berries near harvest.", ["hi"] = "ओलावृष्टि वाले तूफ़ान मिनटों में पत्तियों को फाड़ सकते हैं और फल को चोटिल या फाड़ सकते हैं - खासकर कटाई के समय सेब, अंगूर और बेरी के लिए हानिकारक।", ["ur"] = "اولوں کے ساتھ طوفان منٹوں میں پتیوں کو پھاڑ سکتے ہیں اور پھل کو زخمی یا شگافتہ کر سکتے ہیں - خاص طور پر کٹائی کے وقت سیب، انگور اور بیریوں کے لیے نقصان دہ۔" },
            ["landing.riskHeatTitle"] = new() { ["en"] = "Heat Wave", ["hi"] = "लू", ["ur"] = "گرمی کی لہر" },
            ["landing.riskHeatBody"] = new() { ["en"] = "Sustained high temperatures raise water demand and can scald sun-exposed fruit, stress flowering crops, and reduce fruit/seed set.", ["hi"] = "लगातार उच्च तापमान पानी की मांग बढ़ाता है और धूप में खुले फल को झुलसा सकता है, फूल वाली फ़सलों पर तनाव डाल सकता है, और फल/बीज बनने को कम कर सकता है।", ["ur"] = "مسلسل زیادہ درجہ حرارت پانی کی طلب بڑھاتا ہے اور دھوپ میں کھلے پھل کو جھلسا سکتا ہے، پھول والی فصلوں پر دباؤ ڈال سکتا ہے، اور پھل/بیج بننے کو کم کر سکتا ہے۔" },
            ["landing.riskFrostTitle"] = new() { ["en"] = "Frost", ["hi"] = "पाला", ["ur"] = "پالا" },
            ["landing.riskFrostBody"] = new() { ["en"] = "Near-freezing overnight lows can kill blossoms and young shoots outright, wiping out a season's fruit set in tree fruit and vine crops.", ["hi"] = "रात में हिमांक के करीब तापमान फूलों और नई कोंपलों को तुरंत नष्ट कर सकता है, जिससे फलदार पेड़ों और बेल फ़सलों का पूरा मौसम बर्बाद हो सकता है।", ["ur"] = "رات کو نقطہ انجماد کے قریب درجہ حرارت پھولوں اور نئی کونپلوں کو فوراً ختم کر سکتا ہے، جس سے پھل دار درختوں اور بیل والی فصلوں کا پورا موسم برباد ہو سکتا ہے۔" },
            ["landing.riskWindTitle"] = new() { ["en"] = "Windstorm", ["hi"] = "तेज़ आंधी", ["ur"] = "تیز آندھی" },
            ["landing.riskWindBody"] = new() { ["en"] = "Strong winds strip fruit, snap limbs, lodge tall crops, and tear row covers and trellising loose.", ["hi"] = "तेज़ हवाएँ फल गिरा देती हैं, शाखाएँ तोड़ देती हैं, लंबी फ़सलों को गिरा देती हैं, और रो कवर व ट्रेलिस को ढीला कर देती हैं।", ["ur"] = "تیز ہوائیں پھل گرا دیتی ہیں، شاخیں توڑ دیتی ہیں، لمبی فصلوں کو گرا دیتی ہیں، اور رو کور اور ٹریلس کو ڈھیلا کر دیتی ہیں۔" },
            ["landing.riskRainTitle"] = new() { ["en"] = "Heavy Rainfall", ["hi"] = "भारी वर्षा", ["ur"] = "شدید بارش" },
            ["landing.riskRainBody"] = new() { ["en"] = "Waterlogging suffocates roots and washes away treatments, while wet canopies invite fungal disease.", ["hi"] = "जलभराव जड़ों का दम घोंट देता है और उपचार को बहा देता है, जबकि गीली पत्तियाँ फफूंद रोग को आमंत्रित करती हैं।", ["ur"] = "پانی جمع ہونا جڑوں کا دم گھونٹ دیتا ہے اور علاج کو بہا دیتا ہے، جبکہ گیلی شاخیں پھپھوندی کی بیماری کو دعوت دیتی ہیں۔" },
            ["landing.riskHumidityTitle"] = new() { ["en"] = "High Humidity", ["hi"] = "अधिक नमी", ["ur"] = "زیادہ نمی" },
            ["landing.riskHumidityBody"] = new() { ["en"] = "Prolonged humidity favors fungal and bacterial disease - scab, brown rot, mildew, and blight all spread faster in damp conditions.", ["hi"] = "लंबे समय तक नमी फफूंद और जीवाणु रोगों को बढ़ावा देती है - खुरंट, भूरा सड़न, फफूंदी और झुलसा रोग नम स्थितियों में तेज़ी से फैलते हैं।", ["ur"] = "طویل نمی پھپھوندی اور بیکٹیریا کی بیماریوں کو فروغ دیتی ہے - خارش، بھورا سڑاؤ، پھپھوندی اور جھلساؤ نم حالات میں تیزی سے پھیلتے ہیں۔" },
            ["landing.ctaBannerTitle"] = new() { ["en"] = "Ready to protect your crops?", ["hi"] = "अपनी फ़सल की सुरक्षा के लिए तैयार हैं?", ["ur"] = "اپنی فصل کی حفاظت کے لیے تیار ہیں؟" },
            ["landing.ctaBannerBody"] = new() { ["en"] = "Registration takes less than a minute and advisories start generating right away.", ["hi"] = "पंजीकरण में एक मिनट से भी कम समय लगता है और सलाह तुरंत मिलनी शुरू हो जाती है।", ["ur"] = "رجسٹریشن میں ایک منٹ سے بھی کم وقت لگتا ہے اور مشاورت فوراً ملنا شروع ہو جاتی ہے۔" },
        };
    }
}
