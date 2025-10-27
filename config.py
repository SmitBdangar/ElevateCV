"""
Configuration file for AI Resume Analyzer
Customize settings here without touching main code
"""

# =============================================================================
# SCORING WEIGHTS
# =============================================================================

SCORING_WEIGHTS = {
    'skills': 0.35,          # Skill match importance (35%)
    'experience': 0.25,      # Experience match importance (25%)
    'education': 0.15,       # Education match importance (15%)
    'semantic': 0.15,        # Semantic similarity importance (15%)
    'keywords': 0.10         # Keyword density importance (10%)
}

# =============================================================================
# SCORE THRESHOLDS
# =============================================================================

SCORE_THRESHOLDS = {
    'strong_match': 85,      # 85+ = Strong match
    'good_match': 70,        # 70-84 = Good match
    'moderate_match': 55,    # 55-69 = Moderate match
    'weak_match': 40,        # 40-54 = Weak match
    # Below 40 = Poor match
}

# =============================================================================
# SKILL KEYWORDS (Add your industry-specific skills here)
# =============================================================================

SKILL_CATEGORIES = {
    'programming': [
        'python', 'java', 'javascript', 'typescript', 'c++', 'c#', 'ruby',
        'go', 'rust', 'swift', 'kotlin', 'scala', 'r', 'matlab', 'php',
        'perl', 'bash', 'powershell', 'sql', 'html', 'css'
    ],
    
    'ml_ai': [
        'machine learning', 'deep learning', 'neural networks', 'nlp',
        'natural language processing', 'computer vision', 'reinforcement learning',
        'tensorflow', 'pytorch', 'keras', 'scikit-learn', 'opencv', 'yolo',
        'transformers', 'bert', 'gpt', 'llm', 'large language model',
        'supervised learning', 'unsupervised learning', 'classification',
        'regression', 'clustering', 'neural architecture search'
    ],
    
    'data': [
        'data analysis', 'data science', 'data visualization', 'statistics',
        'pandas', 'numpy', 'matplotlib', 'seaborn', 'plotly', 'tableau',
        'power bi', 'excel', 'spark', 'hadoop', 'etl', 'data pipeline',
        'data warehouse', 'bigquery', 'snowflake', 'redshift'
    ],
    
    'cloud_devops': [
        'aws', 'azure', 'gcp', 'google cloud', 'docker', 'kubernetes', 'k8s',
        'jenkins', 'ci/cd', 'terraform', 'ansible', 'linux', 'unix',
        'bash', 'git', 'github', 'gitlab', 'bitbucket', 'cloudformation',
        'lambda', 'ec2', 'ecs', 'fargate', 'cloud run', 'app engine'
    ],
    
    'databases': [
        'sql', 'nosql', 'mongodb', 'postgresql', 'mysql', 'mariadb',
        'redis', 'elasticsearch', 'cassandra', 'dynamodb', 'firestore',
        'sqlite', 'oracle', 'sql server', 'database design', 'query optimization'
    ],
    
    'web_frontend': [
        'react', 'angular', 'vue.js', 'svelte', 'next.js', 'nuxt.js',
        'html', 'css', 'sass', 'less', 'tailwind', 'bootstrap',
        'webpack', 'vite', 'responsive design', 'accessibility', 'seo'
    ],
    
    'web_backend': [
        'node.js', 'express', 'django', 'flask', 'fastapi', 'spring boot',
        'asp.net', 'rest api', 'graphql', 'microservices', 'serverless',
        'authentication', 'authorization', 'jwt', 'oauth'
    ],
    
    'mobile': [
        'ios', 'android', 'react native', 'flutter', 'swift', 'kotlin',
        'objective-c', 'java', 'xamarin', 'ionic', 'mobile development'
    ],
    
    'soft_skills': [
        'leadership', 'communication', 'teamwork', 'problem solving',
        'agile', 'scrum', 'kanban', 'project management', 'collaboration',
        'mentoring', 'coaching', 'presentation', 'documentation'
    ],
    
    'security': [
        'cybersecurity', 'security', 'encryption', 'penetration testing',
        'vulnerability assessment', 'owasp', 'ssl', 'tls', 'firewall',
        'vpn', 'iam', 'zero trust', 'devsecops'
    ],
    
    'testing': [
        'unit testing', 'integration testing', 'e2e testing', 'test automation',
        'pytest', 'jest', 'selenium', 'cypress', 'tdd', 'bdd', 'qa'
    ]
}

# =============================================================================
# SKILL TAXONOMY (Related skills mapping)
# =============================================================================

SKILL_TAXONOMY = {
    'python': ['pandas', 'numpy', 'scikit-learn', 'django', 'flask', 'fastapi'],
    'machine learning': ['deep learning', 'neural networks', 'tensorflow', 'pytorch', 'keras'],
    'data science': ['machine learning', 'statistics', 'data analysis', 'visualization'],
    'cloud': ['aws', 'azure', 'gcp', 'google cloud'],
    'devops': ['docker', 'kubernetes', 'ci/cd', 'jenkins', 'terraform'],
    'web development': ['react', 'angular', 'vue.js', 'node.js', 'javascript'],
    'backend': ['api', 'rest', 'graphql', 'microservices', 'database'],
    'frontend': ['html', 'css', 'javascript', 'react', 'responsive design'],
    'database': ['sql', 'mysql', 'postgresql', 'mongodb', 'redis'],
    'testing': ['unit testing', 'integration testing', 'pytest', 'jest', 'selenium']
}

# =============================================================================
# UI CONFIGURATION
# =============================================================================

UI_CONFIG = {
    'app_title': '📄 AI Resume Analyzer',
    'app_description': '**Analyze resumes against job descriptions with AI-powered insights**',
    'sidebar_title': '⚙️ Configuration',
    'max_file_size_mb': 10,  # Maximum file size for uploads
    'supported_formats': ['pdf', 'docx', 'txt'],
    'show_debug_info': False,  # Set to True for debugging
}

# =============================================================================
# API CONFIGURATION
# =============================================================================

API_CONFIG = {
    'embedding_model': 'text-embedding-3-small',  # OpenAI embedding model
    'gpt_model': 'gpt-3.5-turbo',  # GPT model for feedback
    'max_tokens_feedback': 1500,  # Max tokens for feedback generation
    'temperature': 0.7,  # Creativity level (0-1)
    'timeout_seconds': 30,  # API timeout
}

# =============================================================================
# ANALYSIS SETTINGS
# =============================================================================

ANALYSIS_SETTINGS = {
    'min_skill_matches_for_good_score': 3,  # Minimum skills to score well
    'experience_weight_multiplier': 1.0,  # Adjust experience importance
    'education_bonus_points': 5,  # Bonus points for relevant education
    'certification_bonus_points': 3,  # Bonus points per certification
    'max_skills_to_display': 15,  # Max skills shown in UI
    'max_missing_skills_to_display': 10,  # Max missing skills shown
}

# =============================================================================
# FEEDBACK TEMPLATES
# =============================================================================

FEEDBACK_TEMPLATES = {
    'strong_match': """
Excellent candidate with strong alignment to the role! 
This resume demonstrates the key qualifications needed.
""",
    
    'good_match': """
Solid candidate with good potential for this role.
Some minor gaps that can likely be addressed.
""",
    
    'moderate_match': """
Candidate shows potential but has several skill gaps.
Consider for roles requiring less experience or with training.
""",
    
    'weak_match': """
Limited alignment with role requirements.
Significant skill gaps would require extensive training.
""",
    
    'poor_match': """
This candidate does not appear to be a good fit for this role.
Consider other positions that better match their background.
"""
}

# =============================================================================
# CUSTOMIZATION TIPS
# =============================================================================

"""
HOW TO CUSTOMIZE THIS FILE:

1. SCORING WEIGHTS:
   - Adjust SCORING_WEIGHTS to change what matters most
   - Must sum to 1.0 (100%)
   - Example: Make skills 50% → 'skills': 0.50

2. ADD SKILLS:
   - Add to SKILL_CATEGORIES under appropriate category
   - Or create new category
   - Use lowercase for consistency

3. SKILL RELATIONSHIPS:
   - Update SKILL_TAXONOMY to connect related skills
   - Helps match "TensorFlow" to "Machine Learning"

4. SCORE THRESHOLDS:
   - Adjust SCORE_THRESHOLDS to be more/less strict
   - Higher values = more selective

5. UI SETTINGS:
   - Change UI_CONFIG for different branding
   - Adjust file size limits
   - Enable/disable features

6. API SETTINGS:
   - Switch to 'gpt-4' for better feedback (more expensive)
   - Adjust temperature for creativity
   - Change max_tokens for longer/shorter feedback

After editing, save and restart the app!
"""

# =============================================================================
# VALIDATION
# =============================================================================

def validate_config():
    """Validate configuration settings"""
    
    # Check scoring weights sum to 1.0
    total_weight = sum(SCORING_WEIGHTS.values())
    if abs(total_weight - 1.0) > 0.01:
        print(f"⚠️  WARNING: Scoring weights sum to {total_weight:.2f}, should be 1.0")
    
    # Check thresholds are in order
    thresholds = sorted(SCORE_THRESHOLDS.values(), reverse=True)
    if thresholds != list(SCORE_THRESHOLDS.values()):
        print("⚠️  WARNING: Score thresholds should be in descending order")
    
    all_skills = []
    for category, skills in SKILL_CATEGORIES.items():
        all_skills.extend(skills)
    
    if len(all_skills) != len(set(all_skills)):
        print("⚠️  WARNING: Duplicate skills found in SKILL_CATEGORIES")
    
    print("✅ Configuration validated")

if __name__ == "__main__":
    validate_config()