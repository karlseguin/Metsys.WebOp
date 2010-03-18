(function($) 
{
    $.fn.rollover = function() 
    {
        return this.each(function() 
        {
            if (this.rollover) { return false; }
            var $image = $(this);
            var r =
            {
                initialize: function() 
                {
                    r.preload();
                    $image.hover(function() { $(this).attr('src', r.overImagePath()); }, function() { $(this).attr('src', r.normalImagePath()); });
                    $image.data('rolloverNormal', $image.attr('src'));
                },
                overImagePath: function() 
                {
                    var src = $image.attr('src').replace(/\./, '_o.');
                    var rel = $image.attr('rel');
                    if (rel) 
                    {
                        src = src.replace(/(\?.*)?$/, '?' + rel);
                    }
                    return src;
                },
                normalImagePath: function() 
                {
                    return $image.data('rolloverNormal');
                },
                preload: function() 
                {
                    $('<img>').attr('src', r.overImagePath());
                }
            }
            this.rollover = r;
            r.initialize();
        });
    };
})(jQuery);