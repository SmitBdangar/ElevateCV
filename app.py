import streamlit as st
from typing import Dict, List, Optional
import re
import json
from dataclasses import dataclass, asdict
import numpy as np
from datetime import datetime
import hashlib
import pandas as pd


# Configure page
st.set_page_config(
    page_title="Saiqen",
    page_icon="",
    layout="wide",
    initial_sidebar_state="expanded"
)

# Custom CSS for better UI
st.markdown("""
<style>
    .main-header {
        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
        padding: 2rem;
        border-radius: 10px;
        color: white;
        text-align: center;
        margin-bottom: 2rem;
    }
    .metric-card {
        background: #f8f9fa;
        padding: 1.5rem;
        border-radius: 8px;
        border-left: 4px solid #667eea;
        margin: 1rem 0;
    }
    .score-excellent { color: #10b981; font-weight: bold; }
    .score-good { color: #3b82f6; font-weight: bold; }
    .score-average { color: #f59e0b; font-weight: bold; }
    .score-poor { color: #ef4444; font-weight: bold; }
    .skill-badge {
        display: inline-block;
        padding: 0.25rem 0.75rem;
        margin: 0.25rem;
        border-radius: 12px;
        background: #e0e7ff;
        color: #4f46e5;
        font-size: 0.875rem;
    }
    .missing-skill {
        background: #fee2e2;
        color: #dc2626;
    }
    .matched-skill {
        background: #d1fae5;
        color: #059669;
    }
    .stTabs [data-baseweb="tab-list"] {
        gap: 2rem;
    }
    .candidate-card {
        border: 1px solid #e5e7eb;
        border-radius: 8px;
        padding: 1rem;
        margin: 0.5rem 0;
        background: white;
        transition: all 0.3s;
    }
    .candidate-card:hover {
        box-shadow: 0 4px 6px rgba(0,0,0,0.1);
        transform: translateY(-2px);
    }
</style>
""", unsafe_allow_html=True)

# Initialize session state
if 'candidates' not in st.session_state:
    st.session_state.candidates = []
if 'analysis_history' not in st.session_state:
    st.session_state.analysis_history = []
if 'current_job' not in st.session_state:
    st.session_state.current_job = None
    
if 'dark_mode' not in st.session_state:
    st.session_state.dark_mode = False

@dataclass
class ResumeData:
    """Structured resume information"""
    candidate_name: str
    email: str
    phone: str
    skills: List[str]
    experience: List[Dict]
    education: List[str]
    certifications: List[str]
    years_of_experience: float
    raw_text: str
    timestamp: str
    
@dataclass
class JobData:
    """Structured job posting information"""
    title: str
    required_skills: List[str]
    preferred_skills: List[str]
    experience_required: str
    education_required: str
    responsibilities: List[str]
    raw_text: str

@dataclass
class AnalysisResult:
    """Complete analysis result"""
    candidate_id: str
    candidate_name: str
    overall_score: float
    skill_score: float
    experience_score: float
    education_score: float
    ats_score: float
    semantic_score: float
    recommendation: str
    strengths: List[str]
    weaknesses: List[str]
    missing_skills: List[str]
    matched_skills: List[str]
    feedback: str
    resume_data: ResumeData
    timestamp: str
    notes: str = ""

class EnhancedResumeParser:
    """Advanced resume parsing with better extraction"""
    
    @staticmethod
    def extract_contact_info(text: str) -> Dict[str, str]:
        """Extract name, email, phone"""
        contact = {'name': 'Unknown', 'email': '', 'phone': ''}
        
        # Extract email
        email_pattern = r'\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b'
        email_match = re.search(email_pattern, text)
        if email_match:
            contact['email'] = email_match.group(0)
        
        # Extract phone
        phone_pattern = r'[\+\(]?[1-9][0-9 .\-\(\)]{8,}[0-9]'
        phone_match = re.search(phone_pattern, text)
        if phone_match:
            contact['phone'] = phone_match.group(0)
        
        # Extract name (first line usually)
        lines = [l.strip() for l in text.split('\n') if l.strip()]
        if lines:
            # Name is often first substantial line
            for line in lines[:5]:
                if len(line.split()) <= 4 and len(line) > 3 and not '@' in line:
                    contact['name'] = line
                    break
        
        return contact
    
    @staticmethod
    def extract_skills(text: str) -> List[str]:
        """Enhanced skill extraction"""
        skill_keywords = {
            'programming': ['python', 'java', 'javascript', 'typescript', 'c\\+\\+', 'c#', 'ruby', 'go', 'rust', 'swift', 'kotlin', 'php', 'r', 'matlab', 'scala'],
            'web': ['html', 'css', 'react', 'angular', 'vue', 'node.js', 'express', 'django', 'flask', 'fastapi', 'spring', 'asp.net'],
            'database': ['sql', 'mysql', 'postgresql', 'mongodb', 'redis', 'cassandra', 'oracle', 'dynamodb', 'sqlite'],
            'cloud': ['aws', 'azure', 'gcp', 'google cloud', 'heroku', 'digitalocean', 'cloudflare'],
            'devops': ['docker', 'kubernetes', 'jenkins', 'gitlab', 'github actions', 'terraform', 'ansible', 'ci/cd'],
            'data': ['pandas', 'numpy', 'spark', 'hadoop', 'kafka', 'airflow', 'dbt', 'tableau', 'power bi', 'excel'],
            'ml_ai': ['machine learning', 'deep learning', 'nlp', 'computer vision', 'tensorflow', 'pytorch', 'scikit-learn', 'keras', 'transformers', 'llm', 'opencv'],
            'mobile': ['ios', 'android', 'react native', 'flutter', 'xamarin', 'swift', 'kotlin'],
            'other': ['git', 'agile', 'scrum', 'jira', 'rest api', 'graphql', 'microservices', 'testing', 'junit', 'selenium']
        }
        
        text_lower = text.lower()
        found_skills = []
        
        for category, skills in skill_keywords.items():
            for skill in skills:
                pattern = r'\b' + skill.replace('+', r'\+') + r'\b'
                if re.search(pattern, text_lower):
                    display_skill = skill.replace('\\+', '+')
                    found_skills.append(display_skill)
        
        return list(set(found_skills))
    
    @staticmethod
    def extract_experience(text: str) -> List[Dict]:
        """Extract work experience with years"""
        experiences = []
        
        # Pattern for job entries with dates
        # Example: "Software Engineer | Jan 2020 - Present"
        job_pattern = r'([A-Z][^|\n]{10,80})\s*[\|•]\s*(\d{4}\s*[-–]\s*(?:\d{4}|Present|Current))'
        matches = re.findall(job_pattern, text, re.IGNORECASE)
        
        for title, dates in matches:
            experiences.append({
                'title': title.strip(),
                'dates': dates.strip()
            })
        
        return experiences[:10]  # Limit to 10 most recent
    
    @staticmethod
    def extract_years_of_experience(text: str, experiences: List[Dict]) -> float:
        """Calculate total years of experience"""
        if not experiences:
            # Look for statements like "5+ years of experience"
            years_pattern = r'(\d+)\+?\s*years?\s+(?:of\s+)?experience'
            match = re.search(years_pattern, text, re.IGNORECASE)
            if match:
                return float(match.group(1))
            return 0.0
        
        total_years = 0.0
        current_year = datetime.now().year
        
        for exp in experiences:
            dates = exp.get('dates', '')
            # Extract years
            year_matches = re.findall(r'\d{4}', dates)
            if len(year_matches) >= 2:
                start_year = int(year_matches[0])
                end_year = int(year_matches[1]) if 'present' not in dates.lower() else current_year
                total_years += (end_year - start_year)
            elif len(year_matches) == 1 and 'present' in dates.lower():
                start_year = int(year_matches[0])
                total_years += (current_year - start_year)
        
        return round(total_years, 1)
    
    @staticmethod
    def extract_education(text: str) -> List[str]:
        """Extract education information"""
        education = []
        
        patterns = [
            r'(?:bachelor|master|phd|doctorate|b\.s\.|m\.s\.|b\.a\.|m\.a\.|mba|b\.tech|m\.tech)[^\n]{0,100}',
            r'(?:university|college|institute)[^\n]{0,100}'
        ]
        
        for pattern in patterns:
            matches = re.findall(pattern, text, re.IGNORECASE)
            education.extend([m.strip() for m in matches])
        
        return list(set(education))[:5]
    
    @staticmethod
    def extract_certifications(text: str) -> List[str]:
        """Extract certifications"""
        cert_keywords = [
            'certified', 'certification', 'certificate', 'aws certified',
            'azure certified', 'google certified', 'pmp', 'cissp', 'cisa'
        ]
        
        certifications = []
        lines = text.split('\n')
        
        for line in lines:
            line_lower = line.lower()
            for keyword in cert_keywords:
                if keyword in line_lower and len(line.strip()) < 150:
                    certifications.append(line.strip())
                    break
        
        return list(set(certifications))[:10]
    
    @classmethod
    def parse(cls, resume_text: str) -> ResumeData:
        """Parse resume with enhanced extraction"""
        contact = cls.extract_contact_info(resume_text)
        skills = cls.extract_skills(resume_text)
        experiences = cls.extract_experience(resume_text)
        years_exp = cls.extract_years_of_experience(resume_text, experiences)
        
        return ResumeData(
            candidate_name=contact['name'],
            email=contact['email'],
            phone=contact['phone'],
            skills=skills,
            experience=experiences,
            education=cls.extract_education(resume_text),
            certifications=cls.extract_certifications(resume_text),
            years_of_experience=years_exp,
            raw_text=resume_text,
            timestamp=datetime.now().isoformat()
        )

class EnhancedJobParser:
    """Enhanced job description parsing"""
    
    @staticmethod
    def extract_title(text: str) -> str:
        """Extract job title"""
        lines = [l.strip() for l in text.split('\n') if l.strip()]
        # First substantial line is often the title
        for line in lines[:5]:
            if 5 < len(line) < 100 and not '@' in line:
                return line
        return "Job Position"
    
    @staticmethod
    def extract_responsibilities(text: str) -> List[str]:
        """Extract job responsibilities"""
        responsibilities = []
        
        # Look for bullet points or numbered lists
        bullet_pattern = r'[•\-\*]\s*([^\n•\-\*]{20,200})'
        matches = re.findall(bullet_pattern, text)
        
        # Filter for responsibility-like content
        resp_keywords = ['develop', 'design', 'implement', 'manage', 'lead', 'collaborate', 'build', 'create']
        for match in matches:
            if any(kw in match.lower() for kw in resp_keywords):
                responsibilities.append(match.strip())
        
        return responsibilities[:10]
    
    @classmethod
    def parse(cls, job_text: str) -> JobData:
        """Parse job description"""
        return JobData(
            title=cls.extract_title(job_text),
            required_skills=EnhancedResumeParser.extract_skills(job_text),
            preferred_skills=[],  # Could be enhanced
            experience_required="",
            education_required="",
            responsibilities=cls.extract_responsibilities(job_text),
            raw_text=job_text
        )

class ATSCompatibilityChecker:
    """Check ATS compatibility of resume"""
    
    @staticmethod
    def check_formatting(text: str) -> Dict[str, any]:
        """Check resume formatting for ATS compatibility"""
        issues = []
        score = 100.0
        
        # Check for tables (problematic for ATS)
        if '|' in text and text.count('|') > 5:
            issues.append("Contains table formatting (may not parse well)")
            score -= 15
        
        # Check for special characters
        special_chars = ['â€¢', '★', '◆', '▪']
        if any(char in text for char in special_chars):
            issues.append("Contains special characters that may not parse correctly")
            score -= 10
        
        # Check for clear sections
        section_keywords = ['experience', 'education', 'skills']
        sections_found = sum(1 for kw in section_keywords if kw.lower() in text.lower())
        if sections_found < 2:
            issues.append("Missing clear section headers (Experience, Education, Skills)")
            score -= 20
        
        # Check length
        word_count = len(text.split())
        if word_count < 200:
            issues.append("Resume is too short (< 200 words)")
            score -= 15
        elif word_count > 1500:
            issues.append("Resume is very long (> 1500 words)")
            score -= 10
        
        # Check for contact info
        if not re.search(r'\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b', text):
            issues.append("Email address not clearly visible")
            score -= 10
        
        recommendations = []
        if score < 80:
            recommendations.append("Use standard section headers (EXPERIENCE, EDUCATION, SKILLS)")
            recommendations.append("Avoid tables, text boxes, and images")
            recommendations.append("Use simple bullet points (• or -)")
            recommendations.append("Save as .docx or .pdf format")
        
        return {
            'score': max(0, score),
            'issues': issues,
            'recommendations': recommendations
        }

class EnhancedScoringEngine:
    """Advanced scoring with multiple factors"""
    
    @staticmethod
    def calculate_skill_match(resume: ResumeData, job: JobData) -> Dict:
        """Calculate comprehensive skill match"""
        resume_skills = set(s.lower() for s in resume.skills)
        job_skills = set(s.lower() for s in job.required_skills)
        
        if not job_skills:
            return {
                'score': 50.0,
                'matched': [],
                'missing': [],
                'extra': list(resume.skills)
            }
        
        matched = resume_skills & job_skills
        missing = job_skills - resume_skills
        extra = resume_skills - job_skills
        
        match_percentage = (len(matched) / len(job_skills)) * 100 if job_skills else 0
        
        return {
            'score': match_percentage,
            'matched': sorted(list(matched)),
            'missing': sorted(list(missing)),
            'extra': sorted(list(extra))
        }
    
    @staticmethod
    def calculate_experience_match(resume: ResumeData, job: JobData) -> float:
        """Score experience match"""
        years = resume.years_of_experience
        
        # Look for required years in job description
        years_pattern = r'(\d+)\+?\s*years?'
        match = re.search(years_pattern, job.raw_text, re.IGNORECASE)
        
        if match:
            required_years = int(match.group(1))
            if years >= required_years:
                return 100.0
            elif years >= required_years * 0.8:
                return 80.0
            elif years >= required_years * 0.5:
                return 60.0
            else:
                return 40.0
        
        # Default scoring based on experience
        if years >= 10:
            return 100.0
        elif years >= 5:
            return 90.0
        elif years >= 3:
            return 75.0
        elif years >= 1:
            return 60.0
        else:
            return 40.0
    
    @staticmethod
    def calculate_education_match(resume: ResumeData, job: JobData) -> float:
        """Score education match"""
        if not resume.education:
            return 50.0
        
        education_text = ' '.join(resume.education).lower()
        
        # Check for degrees
        if 'phd' in education_text or 'doctorate' in education_text:
            return 100.0
        elif 'master' in education_text or 'm.s.' in education_text:
            return 90.0
        elif 'bachelor' in education_text or 'b.s.' in education_text:
            return 80.0
        else:
            return 60.0
    
    @classmethod
    def generate_analysis(
        cls,
        resume: ResumeData,
        job: JobData,
        api_key: Optional[str] = None
    ) -> AnalysisResult:
        """Generate complete analysis"""
        
        # Calculate scores
        skill_analysis = cls.calculate_skill_match(resume, job)
        skill_score = skill_analysis['score']
        experience_score = cls.calculate_experience_match(resume, job)
        education_score = cls.calculate_education_match(resume, job)
        
        # ATS check
        ats_check = ATSCompatibilityChecker.check_formatting(resume.raw_text)
        ats_score = ats_check['score']
        
        # Semantic similarity (simplified - would use embeddings with API)
        semantic_score = 75.0  # Placeholder
        
        # Overall score (weighted average)
        overall_score = (
            skill_score * 0.40 +
            experience_score * 0.25 +
            education_score * 0.15 +
            ats_score * 0.10 +
            semantic_score * 0.10
        )
        
        # Generate recommendation
        if overall_score >= 80:
            recommendation = "🟢 Strong Match - Highly Recommended"
        elif overall_score >= 65:
            recommendation = "🟡 Good Match - Recommended for Interview"
        elif overall_score >= 50:
            recommendation = "🟠 Moderate Match - Consider with Reservations"
        else:
            recommendation = "🔴 Weak Match - Not Recommended"
        
        # Generate strengths and weaknesses
        strengths = []
        weaknesses = []
        
        if skill_score >= 70:
            strengths.append(f"Strong skill match ({len(skill_analysis['matched'])} matching skills)")
        else:
            weaknesses.append(f"Skill gaps ({len(skill_analysis['missing'])} missing skills)")
        
        if experience_score >= 80:
            strengths.append(f"Excellent experience ({resume.years_of_experience} years)")
        elif experience_score < 60:
            weaknesses.append(f"Limited experience ({resume.years_of_experience} years)")
        
        if education_score >= 80:
            strengths.append("Strong educational background")
        
        if ats_score >= 80:
            strengths.append("ATS-friendly resume format")
        else:
            weaknesses.append("Resume may have ATS compatibility issues")
        
        if resume.certifications:
            strengths.append(f"Relevant certifications ({len(resume.certifications)})")
        
        # Generate feedback
        feedback = cls._generate_feedback(resume, job, skill_analysis, ats_check)
        
        # Create unique ID
        candidate_id = hashlib.md5(
            f"{resume.candidate_name}{resume.timestamp}".encode()
        ).hexdigest()[:8]
        
        return AnalysisResult(
            candidate_id=candidate_id,
            candidate_name=resume.candidate_name,
            overall_score=round(overall_score, 1),
            skill_score=round(skill_score, 1),
            experience_score=round(experience_score, 1),
            education_score=round(education_score, 1),
            ats_score=round(ats_score, 1),
            semantic_score=round(semantic_score, 1),
            recommendation=recommendation,
            strengths=strengths,
            weaknesses=weaknesses,
            missing_skills=skill_analysis['missing'],
            matched_skills=skill_analysis['matched'],
            feedback=feedback,
            resume_data=resume,
            timestamp=datetime.now().isoformat()
        )
    
    @staticmethod
    def _generate_feedback(resume: ResumeData, job: JobData, skill_analysis: Dict, ats_check: Dict) -> str:
        """Generate detailed feedback"""
        feedback_parts = []
        
        feedback_parts.append("###Analysis Summary\n")
        
        # Skills feedback
        if skill_analysis['matched']:
            feedback_parts.append(f"**Matched Skills ({len(skill_analysis['matched'])}):** {', '.join(skill_analysis['matched'][:10])}")
        
        if skill_analysis['missing']:
            feedback_parts.append(f"\n**Missing Skills ({len(skill_analysis['missing'])}):** {', '.join(skill_analysis['missing'][:10])}")
            feedback_parts.append("\n*Recommendation:* Consider adding these skills through courses, projects, or highlighting hidden skills.")
        
        # Experience feedback
        feedback_parts.append(f"\n### 💼 Experience: {resume.years_of_experience} years")
        if resume.experience:
            feedback_parts.append(f"- {len(resume.experience)} positions documented")
        
        # ATS feedback
        if ats_check['issues']:
            feedback_parts.append("\n### ⚠️ ATS Compatibility Issues")
            for issue in ats_check['issues'][:3]:
                feedback_parts.append(f"- {issue}")
        
        # Recommendations
        feedback_parts.append("\n### Key Recommendations")
        feedback_parts.append("1. Tailor your resume to highlight matching skills prominently")
        feedback_parts.append("2. Quantify achievements with specific metrics and results")
        feedback_parts.append("3. Use keywords from the job description naturally throughout")
        
        if skill_analysis['missing']:
            feedback_parts.append(f"4. Develop or highlight these missing skills: {', '.join(skill_analysis['missing'][:3])}")
        
        return '\n'.join(feedback_parts)

def display_candidate_card(result: AnalysisResult, idx: int):
    """Display a candidate card in the batch view"""
    
    score_class = (
        "score-excellent" if result.overall_score >= 80 else
        "score-good" if result.overall_score >= 65 else
        "score-average" if result.overall_score >= 50 else
        "score-poor"
    )
    
    with st.container():
        col1, col2, col3, col4 = st.columns([3, 2, 2, 1])
        
        with col1:
            st.markdown(f"**{result.candidate_name}**")
            st.caption(f"ID: {result.candidate_id} | {result.resume_data.email}")
        
        with col2:
            st.markdown(f"<span class='{score_class}'>{result.overall_score:.0f}/100</span>", unsafe_allow_html=True)
            st.caption(f"{len(result.matched_skills)} matched skills")
        
        with col3:
            st.caption(f"{result.resume_data.years_of_experience} yrs exp")
            st.caption(f"{len(result.resume_data.education)} degrees")
        
        with col4:
            if st.button("View", key=f"view_{idx}"):
                st.session_state[f'show_detail_{idx}'] = not st.session_state.get(f'show_detail_{idx}', False)
        
        if st.session_state.get(f'show_detail_{idx}', False):
            with st.expander("Details", expanded=True):
                col_a, col_b = st.columns(2)
                
                with col_a:
                    st.markdown("**Strengths:**")
                    for strength in result.strengths:
                        st.markdown(f"{strength}")
                
                with col_b:
                    st.markdown("**Weaknesses:**")
                    for weakness in result.weaknesses:
                        st.markdown(f"{weakness}")
                
                st.markdown("**Notes:**")
                notes = st.text_area(
                    "Add notes about this candidate",
                    value=result.notes,
                    key=f"notes_{idx}",
                    height=100
                )
                if notes != result.notes:
                    result.notes = notes
        
        st.markdown("---")

def export_results_to_csv(results: List[AnalysisResult]) -> bytes:
    """Export results to CSV"""
    data = []
    for result in results:
        data.append({
            'Candidate ID': result.candidate_id,
            'Name': result.candidate_name,
            'Email': result.resume_data.email,
            'Phone': result.resume_data.phone,
            'Overall Score': result.overall_score,
            'Skill Score': result.skill_score,
            'Experience Score': result.experience_score,
            'Experience (Years)': result.resume_data.years_of_experience,
            'Matched Skills': ', '.join(result.matched_skills),
            'Missing Skills': ', '.join(result.missing_skills),
            'Recommendation': result.recommendation,
            'Notes': result.notes
        })
    
    df = pd.DataFrame(data)
    return df.to_csv(index=False).encode('utf-8')

def main():
    """Main application"""
    
    # Header
    st.markdown("""
    <div class="main-header">
        <h1>🎯 Saiqen Analyzer</h1>
        <p>AI-powered resume analysis with batch processing, ATS checking, and detailed insights</p>
    </div>
    """, unsafe_allow_html=True)
    
    # Sidebar
    with st.sidebar:
        st.header("Settings")
        
        api_key = st.text_input(
            "OpenAI API Key (Optional)",
            type="password",
            help="For enhanced analysis"
        )
        
        st.markdown("---")
        
        # Mode selection
        st.subheader("Analysis Mode")
        mode = st.radio(
            "Select mode:",
            ["Single Resume", "Batch Processing", "Analytics Dashboard"],
            label_visibility="collapsed"
        )
        
        st.markdown("---")
        
        # Quick stats
        st.subheader("Session Stats")
        st.metric("Candidates Analyzed", len(st.session_state.candidates))
        st.metric("Current Job", "Active" if st.session_state.current_job else "None")
        
        if st.session_state.candidates:
            avg_score = np.mean([c.overall_score for c in st.session_state.candidates])
            st.metric("Average Score", f"{avg_score:.1f}")
        
        st.markdown("---")
        
        # Quick actions
        st.subheader("Quick Actions")
        if st.button("Load Sample Data", use_container_width=True):
            st.session_state['load_sample'] = True
            st.rerun()
        
        if st.button("Clear All Data", use_container_width=True):
            st.session_state.candidates = []
            st.session_state.current_job = None
            st.success("Data cleared!")
        
        # st.markdown("---")
        # st.markdown("###Features")
        # st.markdown("""
        # - Batch resume processing
        # - ATS compatibility check
        # - Skill gap analysis
        # - ✅ Experience matching
        # - ✅ Candidate ranking
        # - ✅ Export to CSV
        # - ✅ Comparison view
        # """)
    
    # Load sample data if requested
    if st.session_state.get('load_sample'):
        # Sample data would go here
        st.session_state['load_sample'] = False
    
    # Main content based on mode
    if mode == "Single Resume":
        display_single_resume_mode(api_key)
    elif mode == "Batch Processing":
        display_batch_processing_mode(api_key)
    else:
        display_analytics_dashboard()

def display_single_resume_mode(api_key: Optional[str]):
    """Single resume analysis mode"""
    
    st.subheader("Single Resume Analysis")
    
    # Job description first
    st.markdown("### Step 1: Enter Job Description")
    job_text = st.text_area(
        "Job Description",
        height=200,
        placeholder="Paste the job description here...\n\nInclude:\n- Required skills\n- Experience requirements\n- Responsibilities"
    )
    
    if job_text:
        st.session_state.current_job = EnhancedJobParser.parse(job_text)
        st.success(f"Job loaded: {st.session_state.current_job.title}")
    
    st.markdown("---")
    
    # Resume input
    st.markdown("### Step 2: Enter Resume")
    
    tab1, tab2 = st.tabs(["Paste Text", "Upload File"])
    
    resume_text = ""
    
    with tab1:
        resume_text_paste = st.text_area(
            "Resume Text",
            height=300,
            placeholder="Paste resume text here...",
            key="resume_text_paste"
        )
        if resume_text_paste:
            resume_text = resume_text_paste
    
    with tab2:
        uploaded_file = st.file_uploader(
            "Upload Resume (TXT, PDF, DOCX)",
            type=['txt', 'pdf', 'docx'],
            key="resume_upload_single"
        )
        
        if uploaded_file:
            try:
                if uploaded_file.name.endswith('.txt'):
                    resume_text = uploaded_file.read().decode('utf-8')
                    st.success(f"Loaded: {uploaded_file.name}")
                    with st.expander("Preview"):
                        st.text_area("File content", resume_text[:500] + "...", height=150, disabled=True)
                else:
                    st.info("PDF/DOCX parsing requires additional libraries. Please install PyPDF2 or python-docx, or use the Paste Text tab.")
            except Exception as e:
                st.error(f"Error reading file: {e}")
    
    # Analyze button
    if st.button("Analyze Resume", type="primary", use_container_width=True):
        if not resume_text:
            st.error("Please provide a resume")
            return
        
        if not st.session_state.current_job:
            st.error("Please provide a job description first")
            return
        
        with st.spinner("Analyzing resume..."):
            # Parse resume
            resume = EnhancedResumeParser.parse(resume_text)
            
            # Generate analysis
            result = EnhancedScoringEngine.generate_analysis(
                resume,
                st.session_state.current_job,
                api_key
            )
            
            # Add to candidates
            st.session_state.candidates.append(result)
            
            # Display results
            display_analysis_results(result)

def display_batch_processing_mode(api_key: Optional[str]):
    """Batch resume processing mode"""
    
    st.subheader("Batch Resume Processing")
    
    # Job description
    st.markdown("### Step 1: Set Job Description")
    col1, col2 = st.columns([3, 1])
    
    with col1:
        job_text = st.text_area(
            "Job Description",
            height=150,
            placeholder="Enter job description for batch analysis..."
        )
    
    with col2:
        if st.button("Save Job", use_container_width=True):
            if job_text:
                st.session_state.current_job = EnhancedJobParser.parse(job_text)
                st.success("Job saved!")
    
    if st.session_state.current_job:
        st.info(f"Current Job: **{st.session_state.current_job.title}**")
    
    st.markdown("---")
    
    # Batch resume input
    st.markdown("### Step 2: Add Resumes")
    
    num_resumes = st.number_input("Number of resumes to add", min_value=1, max_value=10, value=3)
    
    resumes_to_process = []
    
    for i in range(num_resumes):
        with st.expander(f"Resume #{i+1}", expanded=(i==0)):
            resume_text = st.text_area(
                f"Resume {i+1} Text",
                height=200,
                key=f"batch_resume_{i}",
                placeholder=f"Paste resume #{i+1} here..."
            )
            if resume_text:
                resumes_to_process.append(resume_text)
    
    # Process batch
    if st.button("Process All Resumes", type="primary", use_container_width=True):
        if not st.session_state.current_job:
            st.error("Please set a job description first")
            return
        
        if not resumes_to_process:
            st.error("Please add at least one resume")
            return
        
        progress_bar = st.progress(0)
        status_text = st.empty()
        
        for idx, resume_text in enumerate(resumes_to_process):
            status_text.text(f"Processing resume {idx+1}/{len(resumes_to_process)}...")
            
            # Parse and analyze
            resume = EnhancedResumeParser.parse(resume_text)
            result = EnhancedScoringEngine.generate_analysis(
                resume,
                st.session_state.current_job,
                api_key
            )
            
            # Add to candidates
            st.session_state.candidates.append(result)
            
            progress_bar.progress((idx + 1) / len(resumes_to_process))
        
        status_text.text("All resumes processed!")
        st.success(f"Processed {len(resumes_to_process)} resumes")
        
        # Clear inputs
        progress_bar.empty()
        status_text.empty()
    
    st.markdown("---")
    
    # Display results
    if st.session_state.candidates:
        st.markdown("###Candidate Rankings")
        
        # Sort options
        col1, col2, col3 = st.columns([2, 2, 1])
        
        with col1:
            sort_by = st.selectbox(
                "Sort by",
                ["Overall Score", "Skill Score", "Experience Score", "Name"]
            )
        
        with col2:
            filter_score = st.slider("Minimum Score", 0, 100, 0)
        
        with col3:
            if st.button("Export CSV", use_container_width=True):
                csv_data = export_results_to_csv(st.session_state.candidates)
                st.download_button(
                    "Download",
                    csv_data,
                    "candidates.csv",
                    "text/csv"
                )
        
        # Sort candidates
        sorted_candidates = sorted(
            st.session_state.candidates,
            key=lambda x: (
                x.overall_score if sort_by == "Overall Score" else
                x.skill_score if sort_by == "Skill Score" else
                x.experience_score if sort_by == "Experience Score" else
                x.candidate_name
            ),
            reverse=(sort_by != "Name")
        )
        
        # Filter by score
        filtered_candidates = [c for c in sorted_candidates if c.overall_score >= filter_score]
        
        st.markdown(f"**Showing {len(filtered_candidates)} of {len(st.session_state.candidates)} candidates**")
        
        # Display cards
        for idx, result in enumerate(filtered_candidates):
            display_candidate_card(result, idx)
    else:
        st.info("No candidates analyzed yet. Process some resumes to see results here.")

def display_analytics_dashboard():
    """Analytics dashboard with insights"""
    
    st.subheader("Analytics Dashboard")
    
    if not st.session_state.candidates:
        st.info("No data available. Analyze some resumes first!")
        return
    
    candidates = st.session_state.candidates
    
    # Overview metrics
    col1, col2, col3, col4 = st.columns(4)
    
    with col1:
        st.metric("Total Candidates", len(candidates))
    
    with col2:
        avg_score = np.mean([c.overall_score for c in candidates])
        st.metric("Average Score", f"{avg_score:.1f}")
    
    with col3:
        strong_candidates = len([c for c in candidates if c.overall_score >= 80])
        st.metric("Strong Matches", strong_candidates)
    
    with col4:
        avg_exp = np.mean([c.resume_data.years_of_experience for c in candidates])
        st.metric("Avg Experience", f"{avg_exp:.1f} yrs")
    
    st.markdown("---")
    
    # Score distribution
    st.markdown("### Score Distribution")
    
    col1, col2 = st.columns(2)
    
    with col1:
        # Create score bins
        scores = [c.overall_score for c in candidates]
        bins = [0, 50, 65, 80, 100]
        labels = ['Weak', 'Moderate', 'Good', 'Strong']
        
        # Count in each bin
        counts = [
            len([s for s in scores if bins[i] <= s < bins[i+1]])
            for i in range(len(bins)-1)
        ]
        
        # Display as metrics
        for label, count in zip(labels, counts):
            st.metric(f"{label} Match", count)
    
    with col2:
        # Top candidates
        st.markdown("**Top 5 Candidates**")
        top_5 = sorted(candidates, key=lambda x: x.overall_score, reverse=True)[:5]
        
        for idx, candidate in enumerate(top_5, 1):
            st.markdown(f"{idx}. **{candidate.candidate_name}** - {candidate.overall_score:.0f}/100")
    
    st.markdown("---")
    
    # Skill analysis
    st.markdown("### Skill Analysis")
    
    col1, col2 = st.columns(2)
    
    with col1:
        st.markdown("**Most Common Skills**")
        
        # Count all skills
        all_skills = {}
        for candidate in candidates:
            for skill in candidate.matched_skills:
                all_skills[skill] = all_skills.get(skill, 0) + 1
        
        # Sort and display top 10
        top_skills = sorted(all_skills.items(), key=lambda x: x[1], reverse=True)[:10]
        
        for skill, count in top_skills:
            percentage = (count / len(candidates)) * 100
            st.markdown(f"- **{skill}**: {count} candidates ({percentage:.0f}%)")
    
    with col2:
        st.markdown("**Most Missing Skills**")
        
        # Count missing skills
        missing_skills = {}
        for candidate in candidates:
            for skill in candidate.missing_skills:
                missing_skills[skill] = missing_skills.get(skill, 0) + 1
        
        # Sort and display top 10
        top_missing = sorted(missing_skills.items(), key=lambda x: x[1], reverse=True)[:10]
        
        for skill, count in top_missing:
            percentage = (count / len(candidates)) * 100
            st.markdown(f"- **{skill}**: {count} candidates ({percentage:.0f}%)")
    
    st.markdown("---")
    
    # Detailed comparison
    st.markdown("### Detailed Comparison")
    
    if len(candidates) >= 2:
        # Select candidates to compare
        candidate_names = [f"{c.candidate_name} ({c.overall_score:.0f})" for c in candidates]
        
        col1, col2 = st.columns(2)
        
        with col1:
            candidate1_idx = st.selectbox("Candidate 1", range(len(candidates)), format_func=lambda x: candidate_names[x])
        
        with col2:
            candidate2_idx = st.selectbox("Candidate 2", range(len(candidates)), index=min(1, len(candidates)-1), format_func=lambda x: candidate_names[x])
        
        if st.button("Compare", use_container_width=True):
            c1 = candidates[candidate1_idx]
            c2 = candidates[candidate2_idx]
            
            col_a, col_b = st.columns(2)
            
            with col_a:
                st.markdown(f"### {c1.candidate_name}")
                st.metric("Overall Score", f"{c1.overall_score:.0f}")
                st.metric("Skills", f"{len(c1.matched_skills)}/{len(c1.matched_skills) + len(c1.missing_skills)}")
                st.metric("Experience", f"{c1.resume_data.years_of_experience} yrs")
                
                st.markdown("**Strengths:**")
                for s in c1.strengths:
                    st.markdown(f"{s}")
            
            with col_b:
                st.markdown(f"### {c2.candidate_name}")
                st.metric("Overall Score", f"{c2.overall_score:.0f}")
                st.metric("Skills", f"{len(c2.matched_skills)}/{len(c2.matched_skills) + len(c2.missing_skills)}")
                st.metric("Experience", f"{c2.resume_data.years_of_experience} yrs")
                
                st.markdown("**Strengths:**")
                for s in c2.strengths:
                    st.markdown(f"{s}")

def display_analysis_results(result: AnalysisResult):
    """Display detailed analysis results"""
    
    st.markdown("---")
    st.header("🎯 Analysis Results")
    
    # Overall score display
    score_emoji = (
        "🟢" if result.overall_score >= 80 else
        "🟡" if result.overall_score >= 65 else
        "🟠" if result.overall_score >= 50 else
        "🔴"
    )
    
    st.markdown(f"""
    <div style="text-align: center; padding: 30px; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); border-radius: 15px; margin-bottom: 30px;">
        <h1 style="color: white; font-size: 72px; margin: 0;">{score_emoji} {result.overall_score:.0f}/100</h1>
        <p style="color: white; font-size: 28px; margin: 10px 0;">{result.recommendation}</p>
        <p style="color: rgba(255,255,255,0.9); font-size: 18px;">Candidate: {result.candidate_name}</p>
    </div>
    """, unsafe_allow_html=True)
    
    # Score breakdown
    st.subheader("Score Breakdown")
    
    col1, col2, col3, col4, col5 = st.columns(5)
    
    with col1:
        st.metric("Skills", f"{result.skill_score:.0f}/100")
    with col2:
        st.metric("Experience", f"{result.experience_score:.0f}/100")
    with col3:
        st.metric("Education", f"{result.education_score:.0f}/100")
    with col4:
        st.metric("ATS Score", f"{result.ats_score:.0f}/100")
    with col5:
        st.metric("Semantic", f"{result.semantic_score:.0f}/100")
    
    st.markdown("---")
    
    # Strengths and Weaknesses
    col1, col2 = st.columns(2)
    
    with col1:
        st.subheader("Strengths")
        for strength in result.strengths:
            st.success(strength)
        
        if not result.strengths:
            st.info("No major strengths identified")
    
    with col2:
        st.subheader("Areas for Improvement")
        for weakness in result.weaknesses:
            st.warning(weakness)
        
        if not result.weaknesses:
            st.info("No major weaknesses identified")
    
    st.markdown("---")
    
    # Skill analysis
    st.subheader("Skill Analysis")
    
    col1, col2 = st.columns(2)
    
    with col1:
        st.markdown("**Matched Skills**")
        if result.matched_skills:
            for skill in result.matched_skills:
                st.markdown(f'<span class="skill-badge matched-skill">{skill}</span>', unsafe_allow_html=True)
        else:
            st.info("No matching skills found")
    
    with col2:
        st.markdown("**Missing Skills**")
        if result.missing_skills:
            for skill in result.missing_skills[:10]:
                st.markdown(f'<span class="skill-badge missing-skill">{skill}</span>', unsafe_allow_html=True)
        else:
            st.success("All required skills present!")
    
    st.markdown("---")
    
    # Detailed feedback
    st.subheader("Detailed Feedback")
    st.markdown(result.feedback)
    
    st.markdown("---")
    
    # Candidate details
    with st.expander("Candidate Details"):
        col1, col2 = st.columns(2)
        
        with col1:
            st.markdown(f"**Name:** {result.resume_data.candidate_name}")
            st.markdown(f"**Email:** {result.resume_data.email}")
            st.markdown(f"**Phone:** {result.resume_data.phone}")
            st.markdown(f"**Experience:** {result.resume_data.years_of_experience} years")
        
        with col2:
            st.markdown(f"**Education:** {len(result.resume_data.education)} entries")
            st.markdown(f"**Certifications:** {len(result.resume_data.certifications)}")
            st.markdown(f"**Total Skills:** {len(result.resume_data.skills)}")
            st.markdown(f"**Analysis Date:** {result.timestamp[:10]}")
    
    # Export options
    st.markdown("---")
    st.subheader("Export Report")
    
    col1, col2 = st.columns(2)
    
    with col1:
        # Generate report
        report = f"""# Resume Analysis Report

## Candidate: {result.candidate_name}
**Overall Score:** {result.overall_score:.0f}/100
**Recommendation:** {result.recommendation}

## Score Breakdown
- Skill Match: {result.skill_score:.0f}/100
- Experience: {result.experience_score:.0f}/100
- Education: {result.education_score:.0f}/100
- ATS Compatibility: {result.ats_score:.0f}/100

## Strengths
{chr(10).join('- ' + s for s in result.strengths)}

## Areas for Improvement
{chr(10).join('- ' + w for w in result.weaknesses)}

## Skill Analysis
**Matched Skills:** {', '.join(result.matched_skills)}
**Missing Skills:** {', '.join(result.missing_skills)}

## Detailed Feedback
{result.feedback}

---
Report generated on {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}
"""
        
        st.download_button(
            "Download as Markdown",
            report,
            file_name=f"analysis_{result.candidate_id}.md",
            mime="text/markdown",
            use_container_width=True
        )
    
    with col2:
        # Export as JSON
        json_data = json.dumps(asdict(result), indent=2, default=str)
        st.download_button(
            "Download as JSON",
            json_data,
            file_name=f"analysis_{result.candidate_id}.json",
            mime="application/json",
            use_container_width=True
        )

if __name__ == "__main__":
    main()